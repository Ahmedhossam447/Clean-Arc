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
    public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, Result<CheckoutOrderResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _config;
        private readonly ILogger<CheckoutOrderCommandHandler> _logger;

        public CheckoutOrderCommandHandler(
            IUnitOfWork unitOfWork,
            IPaymentService paymentService,
            IConfiguration config,
            ILogger<CheckoutOrderCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _config = config;
            _logger = logger;
        }

        public async Task<Result<CheckoutOrderResponse>> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. Load order with items
            var orders = await _unitOfWork.Repository<Order>()
                .GetAsync(o => o.Id == request.OrderId, cancellationToken, o => o.OrderItems);

            var order = orders.FirstOrDefault();
            if (order == null)
                return Order.Errors.NotFound;

            if (order.BuyerId != request.UserId)
                return Order.Errors.Unauthorized;

            if (order.Status != "Pending")
                return Order.Errors.AlreadyProcessed;

            if (!order.OrderItems.Any())
                return Order.Errors.EmptyOrder;

            // 2. Re-validate stock & recalculate subtotal from actual items
            decimal subtotal = 0;
            var productRepo = _unitOfWork.Repository<Product>();
            foreach (var item in order.OrderItems)
            {
                var product = await productRepo.GetByIdAsync(item.ProductId, cancellationToken);
                if (product == null)
                    return Product.Errors.NotFound;

                if (product.StockQuantity < item.Quantity)
                    return Order.Errors.InsufficientStock;

                subtotal += item.Price * item.Quantity;
            }

            order.Subtotal = subtotal;

            // 3. Create PaymentTransaction + call Paymob
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var transaction = new PaymentTransaction
                {
                    PaymobOrderId = 0,
                    UserId = request.UserId,
                    UserEmail = request.UserEmail,
                    Amount = subtotal,
                    Currency = "EGP",
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<PaymentTransaction>().AddAsync(transaction);

                order.PaymentTransaction = transaction;
                _unitOfWork.Repository<Order>().Update(order);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // 4. Call Paymob
                long amountCents = (long)(order.Subtotal * 100);
                var authToken = await _paymentService.GetAuthTokenAsync();
                var paymobOrderId = await _paymentService.CreateOrderAsync(authToken, amountCents);

                transaction.PaymobOrderId = paymobOrderId;
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var billingData = new
                {
                    first_name = "User",
                    last_name = "Test",
                    email = request.UserEmail ?? "N/A",
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

                // 5. Build payment URL
                string iframeId = _config["Paymob:IframeId"]
                    ?? throw new InvalidOperationException("Paymob:IframeId is not configured.");

                string paymentUrl = $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentKey}";

                return new CheckoutOrderResponse
                {
                    OrderId = order.Id,
                    Subtotal = order.Subtotal,
                    Currency = "EGP",
                    PaymentUrl = paymentUrl
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Checkout failed for order {OrderId}", request.OrderId);
                return Result<CheckoutOrderResponse>.Failure(PaymentTransaction.Errors.Failed);
            }
        }
    }
}
