using CleanArc.Core.Primitives;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Request
{
    public class DeleteRequestCommand : IRequest<Result>
    {
        public int RequestId { get; set; }

        [JsonIgnore]
        public string UserId { get; set; } = string.Empty;
    }
}
