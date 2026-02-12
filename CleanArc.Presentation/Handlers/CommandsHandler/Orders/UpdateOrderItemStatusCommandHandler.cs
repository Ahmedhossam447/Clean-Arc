using CleanArc.Application.Commands.Order;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.CommandsHandler.Orders
{
    public class UpdateOrderItemStatusCommandHandler : IRequestHandler<UpdateOrderItemStatusCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        private static readonly string[] AllowedStatuses = { "Processing", "Shipped", "Delivered" };

        public UpdateOrderItemStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateOrderItemStatusCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate status value
            if (!AllowedStatuses.Contains(request.Status))
                return new Error("OrderItem.InvalidStatus", $"Status must be one of: {string.Join(", ", AllowedStatuses)}.");

            // 2. Load the order with items
            var orders = await _unitOfWork.Repository<Order>()
                .GetAsync(o => o.Id == request.OrderId, cancellationToken, o => o.OrderItems);

            var order = orders.FirstOrDefault();
            if (order == null)
                return Order.Errors.NotFound;

            // 3. Only paid orders can have shipment updates
            if (order.Status != "PaymentReceived")
                return new Error("Order.NotPaid", "Shipment status can only be updated for paid orders.");

            // 4. Find the item
            var item = order.OrderItems.FirstOrDefault(oi => oi.Id == request.ItemId);
            if (item == null)
                return new Error("OrderItem.NotFound", "The order item was not found.");

            // 5. Only the shelter who owns this item can update it
            if (item.ShelterId != request.ShelterId)
                return Order.Errors.Unauthorized;

            // 6. Update status
            item.Status = request.Status;
            _unitOfWork.Repository<OrderItem>().Update(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
