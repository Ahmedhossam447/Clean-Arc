using CleanArc.Application.Contracts.Responses.Request;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Commands.Request
{
    public class CreateRequestCommand : IRequest<Result<CreateRequestResponse>>
    {
        public int AnimalId { get; set; }
        public string RequesterId { get; set; } = string.Empty;
    }
}
