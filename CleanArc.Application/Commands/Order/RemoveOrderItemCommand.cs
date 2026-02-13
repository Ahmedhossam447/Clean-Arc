using CleanArc.Core.Primitives;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Order
{
    public class RemoveOrderItemCommand : IRequest<Result>
    {
        [JsonIgnore]
        public int OrderId { get; set; }

        [JsonIgnore]
        public int ItemId { get; set; }

        [JsonIgnore]
        public string UserId { get; set; } = string.Empty;
    }
}
