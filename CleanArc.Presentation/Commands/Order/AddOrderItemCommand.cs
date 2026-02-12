using CleanArc.Application.Contracts.Responses.Order;
using CleanArc.Core.Primitives;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Order
{
    public class AddOrderItemCommand : IRequest<Result<OrderItemResponse>>
    {
        [JsonIgnore]
        public int OrderId { get; set; }

        [JsonIgnore]
        public string UserId { get; set; } = string.Empty;

        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
