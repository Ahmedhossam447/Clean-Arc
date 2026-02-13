using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Commands.Request;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Requests
{
    public class AcceptRequestCommandHandler : IRequestHandler<AcceptRequestCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;
        private readonly IMediator _mediator;
        private readonly INotificationService _notificationService;

        public AcceptRequestCommandHandler(
            IUnitOfWork unitOfWork,
            IDistributedCache cache,
            IMediator mediator,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _mediator = mediator;
            _notificationService = notificationService;
        }

        public async Task<Result> Handle(AcceptRequestCommand command, CancellationToken cancellationToken)
        {
            var requestRepo = _unitOfWork.Repository<Request>();
            var request = await requestRepo.GetByIdAsync(command.RequestId, cancellationToken);

            if (request == null)
                return Result.Failure(Request.Errors.NotFound);

            if (request.OwnerId != command.OwnerId)
                return Result.Failure(Request.Errors.Unauthorized);

            if (request.Status == "Approved")
                return Result.Failure(Request.Errors.AlreadyApproved);

            var adoptCommand = new AdoptAnimalCommand
            {
                AnimalId = request.AnimalId,
                AdopterId = request.RequesterId
            };

            var adoptResult = await _mediator.Send(adoptCommand, cancellationToken);

            if (adoptResult.IsFailure)
                return Result.Failure(adoptResult.Error);

            request.Status = "Approved";
            requestRepo.Update(request);
            await _unitOfWork.SaveChangesAsync();
            await _notificationService.SendNotificationToUserAsync(request.RequesterId, "RequestApproved", new { RequestId = request.Id, AnimalId = request.AnimalId });

            await _cache.RemoveAsync($"requests:user:{request.RequesterId}");

            return Result.Success();
        }
    }
}
