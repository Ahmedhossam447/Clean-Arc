using CleanArc.Application.Commands.Order;
using CleanArc.Application.Contracts.Responses.Order;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.CommandsHandler.Orders
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreateOrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate items not empty
            if (request.Items == null || request.Items.Count == 0)
                return Order.Errors.EmptyOrder;

            // 2. Merge duplicate ProductIds
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
                    return Product.Errors.NotFound;

                if (product.StockQuantity < item.Quantity)
                    return Order.Errors.InsufficientStock;

                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    ProductName = product.Name,
                    PictureUrl = product.ImageUrl,
                    Price = product.Price,
                    ShelterId = product.ShelterId
                });

                subtotal += product.Price * item.Quantity;
            }

            // 4. Save order to DB — no payment yet
            var order = new Order
            {
                BuyerId = request.CustomerId,
                OrderDate = DateTimeOffset.UtcNow,
                Status = "Pending",
                Subtotal = subtotal,
                OrderItems = orderItems
            };

            await _unitOfWork.Repository<Order>().AddAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateOrderResponse
            {
                OrderId = order.Id,
                Subtotal = subtotal,
                Currency = "EGP",
                Status = order.Status,
                Items = orderItems.Select(oi => new OrderItemResponse
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    Price = oi.Price,
                    Quantity = oi.Quantity
                }).ToList()
            };
        }
    }
}
