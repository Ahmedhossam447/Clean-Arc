using CleanArc.Application.Commands.Request;
using CleanArc.Application.Contracts.Responses.Request;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Requests
{
    public class UpdateRequestCommandHandler : IRequestHandler<UpdateRequestCommand, Result<RequestResponse>>
    {
        private readonly IRepository<Request> _requestRepository;
        private readonly IDistributedCache _cache;

        public UpdateRequestCommandHandler(IRepository<Request> requestRepository, IDistributedCache cache)
        {
            _requestRepository = requestRepository;
            _cache = cache;
        }

        public async Task<Result<RequestResponse>> Handle(UpdateRequestCommand command, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetByIdAsync(command.RequestId, cancellationToken);

            if (request == null)
                return Request.Errors.NotFound;

            var oldRequesterId = request.Useridreq;

            if (!string.IsNullOrEmpty(command.OwnerId))
                request.Userid = command.OwnerId;

            if (!string.IsNullOrEmpty(command.RequesterId))
                request.Useridreq = command.RequesterId;

            if (command.AnimalId.HasValue)
                request.AnimalId = command.AnimalId.Value;

            if (!string.IsNullOrEmpty(command.Status))
                request.Status = command.Status;

            _requestRepository.Update(request);
            await _requestRepository.SaveChangesAsync();

            // Cache invalidation after write - don't use cancellationToken
            await _cache.RemoveAsync($"requests:user:{oldRequesterId}");
            if (oldRequesterId != request.Useridreq)
                await _cache.RemoveAsync($"requests:user:{request.Useridreq}");

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
