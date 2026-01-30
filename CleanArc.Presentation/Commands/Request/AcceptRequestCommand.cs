using CleanArc.Core.Primitives;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Request
{
    public class AcceptRequestCommand : IRequest<Result>
    {
        [JsonIgnore]
        public int RequestId { get; set; }

        [JsonIgnore]
        public string OwnerId { get; set; } = string.Empty;
    }
}
