using CleanArc.Application.Contracts.Responses.Request;
using CleanArc.Application.Queries.Request;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.Requests
{
    public class GetReceivedRequestsQueryHandler : IRequestHandler<GetReceivedRequestsQuery, List<RequestResponse>>
    {
        private readonly IRepository<Request> _requestRepository;

        public GetReceivedRequestsQueryHandler(IRepository<Request> requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<List<RequestResponse>> Handle(GetReceivedRequestsQuery query, CancellationToken cancellationToken)
        {
            var requests = await _requestRepository.GetAsync(r => r.Userid == query.OwnerId, cancellationToken);

            return requests.Select(r => new RequestResponse
            {
                Reqid = r.Reqid,
                OwnerId = r.Userid,
                RequesterId = r.Useridreq,
                AnimalId = r.AnimalId,
                AnimalName = r.Animal?.Name,
                Status = r.Status
            }).ToList();
        }
    }
}
