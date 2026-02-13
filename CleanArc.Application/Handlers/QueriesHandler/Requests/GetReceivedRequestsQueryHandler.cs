using CleanArc.Application.Contracts.Responses.Request;
using CleanArc.Application.Queries.Request;
using CleanArc.Core.Entities;
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
            var requests = await _unitOfWork.Repository<Request>().GetAsync(r => r.OwnerId == query.OwnerId, cancellationToken);

            return requests.Select(r => new RequestResponse
            {
                Id = r.Id,
                OwnerId = r.OwnerId,
                RequesterId = r.RequesterId,
                AnimalId = r.AnimalId,
                AnimalName = r.Animal?.Name,
                Status = r.Status
            }).ToList();
        }
    }
}
