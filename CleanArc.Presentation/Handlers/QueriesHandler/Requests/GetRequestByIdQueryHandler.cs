using CleanArc.Application.Contracts.Responses.Request;
using CleanArc.Application.Queries.Request;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.Requests
{
    public class GetRequestByIdQueryHandler : IRequestHandler<GetRequestByIdQuery, Result<RequestResponse>>
    {
        private readonly IRepository<Core.Entites.Request> _requestRepository;

        public GetRequestByIdQueryHandler(IRepository<Core.Entites.Request> requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<Result<RequestResponse>> Handle(GetRequestByIdQuery query, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetByIdAsync(query.RequestId);

            if (request == null)
                return Core.Entites.Request.Errors.NotFound;

            return new RequestResponse
            {
                Reqid = request.Reqid,
                OwnerId = request.Userid,
                RequesterId = request.Useridreq,
                AnimalId = request.AnimalId,
                AnimalName = request.Animal?.Name,
                Status = request.Status
            };
        }
    }
}
