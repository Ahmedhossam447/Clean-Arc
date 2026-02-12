using CleanArc.Application.Commands.Order;
using CleanArc.Application.Contracts.Responses.Order;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CleanArc.Application.Handlers.CommandsHandler.Orders
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _config;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(
            IUnitOfWork unitOfWork,
            IPaymentService paymentService,
            IConfiguration config,
            ILogger<CreateOrderCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _config = config;
            _logger = logger;
        }

        public async Task<Result<CreateOrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate items not empty
            if (request.Items == null || request.Items.Count == 0)
            {
                return Order.Errors.EmptyOrder;
            }

            // 2. Merge duplicate ProductIds (e.g. [{Id:1, Qty:3}, {Id:1, Qty:2}] → [{Id:1, Qty:5}])
            var mergedItems = request.Items
                .GroupBy(i => i.ProductId)
                .Select(g => new { ProductId = g.Key, Quantity = g.Sum(i => i.Quantity) })
                .ToList();

            var orderItems = new List<OrderItem>();
            decimal subtotal = 0;
            var productRepo = _unitOfWork.Repository<Product>();

            // 3. Validate products & build order items
            foreach (var item in mergedItems)
            {
                var product = await productRepo.GetByIdAsync(item.ProductId, cancellationToken);
                if (product == null)
                {
                    return Product.Errors.NotFound;
                }

                if (product.StockQuantity < item.Quantity)
                {
                    return Order.Errors.InsufficientStock;
                }

                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    ProductName = product.Name,
                    PictureUrl = product.ImageUrl,
                    Price = product.Price,
                    ShelterId = product.ShelterId
                };

                orderItems.Add(orderItem);
                subtotal += product.Price * item.Quantity;
            }

            // 4. Save Order + PaymentTransaction to DB FIRST
            //    If Paymob fails later, we rollback. This prevents orphaned Paymob orders.
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var transaction = new PaymentTransaction
                {
                    PaymobOrderId = 0, // Will be updated after Paymob call
                    UserId = request.CustomerId,
                    UserEmail = request.CustomerEmail,
                    Amount = subtotal,
                    Currency = "EGP",
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<PaymentTransaction>().AddAsync(transaction);

                var order = new Order
                {
                    BuyerId = request.CustomerId,
                    OrderDate = DateTimeOffset.UtcNow,
                    Status = "Pending",
                    Subtotal = subtotal,
                    OrderItems = orderItems,
                    PaymentTransaction = transaction
                };

                await _unitOfWork.Repository<Order>().AddAsync(order);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // 5. Call Paymob API
                long amountCents = (long)(subtotal * 100);
                var authToken = await _paymentService.GetAuthTokenAsync();
                var paymobOrderId = await _paymentService.CreateOrderAsync(authToken, amountCents);

                // Update the PaymentTransaction with the real Paymob order ID
                transaction.PaymobOrderId = paymobOrderId;
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var billingData = new
                {
                    first_name = "User",
                    last_name = "Test",
                    email = request.CustomerEmail ?? "N/A",
                    phone_number = "01000000000",
                    apartment = "NA",
                    floor = "NA",
                    street = "NA",
                    building = "NA",
                    shipping_method = "NA",
                    postal_code = "NA",
                    city = "Cairo",
                    country = "EG",
                    state = "Cairo"
                };

                var paymentKey = await _paymentService.GetPaymentKeyAsync(authToken, paymobOrderId, amountCents, billingData);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                // 6. Build response
                string iframeId = _config["Paymob:IframeId"]
                    ?? throw new InvalidOperationException("Paymob:IframeId is not configured.");

                string paymentUrl = $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentKey}";

                var response = new CreateOrderResponse
                {
                    OrderId = order.Id,
                    Subtotal = subtotal,
                    Currency = "EGP",
                    Status = order.Status,
                    PaymentUrl = paymentUrl,
                    Items = orderItems.Select(oi => new OrderItemResponse
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        Price = oi.Price,
                        Quantity = oi.Quantity
                    }).ToList()
                };

                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Failed to create order for user {UserId}", request.CustomerId);
                return Result<CreateOrderResponse>.Failure(PaymentTransaction.Errors.Failed);
            }
        }
    }
}
