using CleanArc.Core.Primitives;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Order
{
    public class UpdateOrderItemStatusCommand : IRequest<Result>
    {
        [JsonIgnore]
        public int OrderId { get; set; }

        [JsonIgnore]
        public int ItemId { get; set; }

        [JsonIgnore]
        public string ShelterId { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}
