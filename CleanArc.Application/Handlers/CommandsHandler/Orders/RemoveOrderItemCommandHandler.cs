using CleanArc.Application.Commands.Order;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.CommandsHandler.Orders
{
    public class RemoveOrderItemCommandHandler : IRequestHandler<RemoveOrderItemCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveOrderItemCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RemoveOrderItemCommand request, CancellationToken cancellationToken)
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

            // 2. Find the item
            var item = order.OrderItems.FirstOrDefault(oi => oi.Id == request.ItemId);
            if (item == null)
                return new Error("OrderItem.NotFound", "The order item was not found.");

            // 3. Remove item
            await _unitOfWork.Repository<OrderItem>().Delete(item.Id);

            // 4. Recalculate subtotal
            order.Subtotal = order.OrderItems
                .Where(oi => oi.Id != request.ItemId)
                .Sum(oi => oi.Price * oi.Quantity);

            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
