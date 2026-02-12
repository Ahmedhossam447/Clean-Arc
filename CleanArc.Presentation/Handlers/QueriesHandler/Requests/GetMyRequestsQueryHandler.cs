using CleanArc.Application.Contracts.Responses.Request;
using CleanArc.Application.Queries.Request;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.Requests
{
    public class GetMyRequestsQueryHandler : IRequestHandler<GetMyRequestsQuery, List<RequestResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMyRequestsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<RequestResponse>> Handle(GetMyRequestsQuery query, CancellationToken cancellationToken)
        {
            var requests = await _unitOfWork.Repository<Core.Entites.Request>().GetAsync(r => r.Useridreq == query.UserId, cancellationToken);

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
