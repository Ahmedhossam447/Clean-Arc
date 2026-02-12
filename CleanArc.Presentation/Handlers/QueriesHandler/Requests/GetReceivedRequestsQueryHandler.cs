using CleanArc.Application.Contracts.Responses.Request;
using CleanArc.Application.Queries.Request;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.Requests
{
    public class GetReceivedRequestsQueryHandler : IRequestHandler<GetReceivedRequestsQuery, List<RequestResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetReceivedRequestsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<RequestResponse>> Handle(GetReceivedRequestsQuery query, CancellationToken cancellationToken)
        {
            var requests = await _unitOfWork.Repository<Request>().GetAsync(r => r.Userid == query.OwnerId, cancellationToken);

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
