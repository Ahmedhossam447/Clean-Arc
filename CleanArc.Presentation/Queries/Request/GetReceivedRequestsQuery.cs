using CleanArc.Application.Contracts.Responses.Request;
using MediatR;

namespace CleanArc.Application.Queries.Request
{
    public class GetReceivedRequestsQuery : IRequest<List<RequestResponse>>
    {
        public string OwnerId { get; set; } = string.Empty;
    }
}
