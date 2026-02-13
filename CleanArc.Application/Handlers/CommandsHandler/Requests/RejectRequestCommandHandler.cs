using CleanArc.Application.Commands.Request;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Requests
{
    public class RejectRequestCommandHandler : IRequestHandler<RejectRequestCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;
        private readonly INotificationService _notificationService;

        public RejectRequestCommandHandler(
            IUnitOfWork unitOfWork,
            IDistributedCache cache,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _notificationService = notificationService;
        }

        public async Task<Result> Handle(RejectRequestCommand command, CancellationToken cancellationToken)
        {
            var requestRepo = _unitOfWork.Repository<Request>();
            var request = await requestRepo.GetByIdAsync(command.RequestId, cancellationToken);

            if (request == null)
                return Result.Failure(Request.Errors.NotFound);

            if (request.OwnerId != command.OwnerId)
                return Result.Failure(Request.Errors.Unauthorized);

            var requesterId = request.RequesterId;

            await requestRepo.Delete(request.Id);
            await _unitOfWork.SaveChangesAsync();
            await _notificationService.SendNotificationToUserAsync(requesterId, "RequestRejected", new { RequestId = request.Id, AnimalId = request.AnimalId });

            await _cache.RemoveAsync($"requests:user:{requesterId}");

            return Result.Success();
        }
    }
}
