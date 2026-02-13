using CleanArc.Application.Contracts.Responses.Request;
using CleanArc.Application.Queries.Request;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.Requests
{
    public class GetRequestByIdQueryHandler : IRequestHandler<GetRequestByIdQuery, Result<RequestResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRequestByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<RequestResponse>> Handle(GetRequestByIdQuery query, CancellationToken cancellationToken)
        {
            var requests = await _unitOfWork.Repository<Request>().GetAsync(
                r => r.Id == query.RequestId,
                cancellationToken,
                r => r.Animal!);

            var request = requests.FirstOrDefault();

            if (request == null)
                return Core.Entities.Request.Errors.NotFound;

            return new RequestResponse
            {
                Id = request.Id,
                OwnerId = request.OwnerId,
                RequesterId = request.RequesterId,
                AnimalId = request.AnimalId,
                AnimalName = request.Animal?.Name,
                Status = request.Status
            };
        }
    }
}
