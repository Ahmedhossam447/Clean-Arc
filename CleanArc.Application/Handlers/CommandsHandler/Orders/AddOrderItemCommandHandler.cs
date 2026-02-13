using CleanArc.Application.Commands.Order;
using CleanArc.Application.Contracts.Responses.Order;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.CommandsHandler.Orders
{
    public class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, Result<OrderItemResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddOrderItemCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<OrderItemResponse>> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
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

            // 2. Validate product
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
                return Product.Errors.NotFound;

            if (product.StockQuantity < request.Quantity)
                return Order.Errors.InsufficientStock;

            // 3. Check if this product already exists in the order — if so, increase quantity
            var existingItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == request.ProductId);

            if (existingItem != null)
            {
                int newQuantity = existingItem.Quantity + request.Quantity;
                if (product.StockQuantity < newQuantity)
                    return Order.Errors.InsufficientStock;

                existingItem.Quantity = newQuantity;
                _unitOfWork.Repository<OrderItem>().Update(existingItem);
            }
            else
            {
                var newItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    ProductName = product.Name,
                    PictureUrl = product.ImageUrl,
                    Price = product.Price,
                    ShelterId = product.ShelterId
                };

                await _unitOfWork.Repository<OrderItem>().AddAsync(newItem);
                existingItem = newItem;
            }

            // 4. Recalculate subtotal
            order.Subtotal = order.OrderItems
                .Where(oi => oi.ProductId != request.ProductId)
                .Sum(oi => oi.Price * oi.Quantity)
                + existingItem.Price * existingItem.Quantity;

            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new OrderItemResponse
            {
                ProductId = existingItem.ProductId,
                ProductName = existingItem.ProductName,
                Price = existingItem.Price,
                Quantity = existingItem.Quantity
            };
        }
    }
}
