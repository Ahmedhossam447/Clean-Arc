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
        private readonly INotificationService _notificationService;

        public RejectRequestCommandHandler(
            IRepository<Request> requestRepository,
            IDistributedCache cache,
            INotificationService notificationService)
        {
            _requestRepository = requestRepository;
            _cache = cache;
            _notificationService = notificationService;
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
            await _notificationService.SendNotificationAsync(requesterId, "RequestRejected", new { RequestId = request.Reqid, AnimalId = request.AnimalId });

            await _cache.RemoveAsync($"requests:user:{requesterId}", cancellationToken);

            return Result.Success();
        }
    }
}
