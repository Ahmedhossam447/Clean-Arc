using CleanArc.Application.Contracts.Responses.Order;
using CleanArc.Core.Primitives;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Order
{
    public class CheckoutOrderCommand : IRequest<Result<CheckoutOrderResponse>>
    {
        [JsonIgnore]
        public int OrderId { get; set; }

        [JsonIgnore]
        public string UserId { get; set; } = string.Empty;

        [JsonIgnore]
        public string? UserEmail { get; set; }
    }
}
