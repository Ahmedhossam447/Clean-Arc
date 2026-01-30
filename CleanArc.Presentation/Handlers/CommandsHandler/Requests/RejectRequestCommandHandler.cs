using CleanArc.Application.Commands.Request;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Requests
{
    public class RejectRequestCommandHandler : IRequestHandler<RejectRequestCommand, Result>
    {
        private readonly IRepository<Request> _requestRepository;
        private readonly IDistributedCache _cache;

        public RejectRequestCommandHandler(
            IRepository<Request> requestRepository,
            IDistributedCache cache)
        {
            _requestRepository = requestRepository;
            _cache = cache;
        }

        public async Task<Result> Handle(RejectRequestCommand command, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetByIdAsync(command.RequestId);

            if (request == null)
                return Result.Failure(Request.Errors.NotFound);

            if (request.Userid != command.OwnerId)
                return Result.Failure(Request.Errors.Unauthorized);

            var requesterId = request.Useridreq;

            await _requestRepository.Delete(request.Reqid);
            await _requestRepository.SaveChangesAsync();

            await _cache.RemoveAsync($"requests:user:{requesterId}", cancellationToken);

            // TODO: Send SignalR notification to requester

            return Result.Success();
        }
    }
}
