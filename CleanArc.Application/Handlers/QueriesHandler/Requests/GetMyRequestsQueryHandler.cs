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
            var requests = await _unitOfWork.Repository<Core.Entities.Request>().GetAsync(r => r.RequesterId == query.UserId, cancellationToken);

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
