using CleanArc.Application.Contracts.Responses.Request;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Queries.Request
{
    public class GetRequestByIdQuery : IRequest<Result<RequestResponse>>
    {
        public int RequestId { get; set; }
    }
}
