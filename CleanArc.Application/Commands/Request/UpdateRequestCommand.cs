using CleanArc.Application.Contracts.Responses.Request;
using CleanArc.Core.Primitives;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Request
{
    public class UpdateRequestCommand : IRequest<Result<RequestResponse>>
    {
        [JsonIgnore]
        public int RequestId { get; set; }
        public string? OwnerId { get; set; }
        public string? RequesterId { get; set; }
        public int? AnimalId { get; set; }
        public string? Status { get; set; }
    }
}
