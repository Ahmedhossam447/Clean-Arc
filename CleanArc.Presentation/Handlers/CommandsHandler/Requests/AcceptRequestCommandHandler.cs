using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Commands.Request;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Requests
{
    public class AcceptRequestCommandHandler : IRequestHandler<AcceptRequestCommand, Result>
    {
        private readonly IRepository<Request> _requestRepository;
        private readonly IDistributedCache _cache;
        private readonly IMediator _mediator;
        private readonly INotificationService _notificationService;

        public AcceptRequestCommandHandler(
            IRepository<Request> requestRepository,
            IDistributedCache cache,
            IMediator mediator,
            INotificationService notificationService)
        {
            _requestRepository = requestRepository;
            _cache = cache;
            _mediator = mediator;
            _notificationService = notificationService;
        }

        public async Task<Result> Handle(AcceptRequestCommand command, CancellationToken cancellationToken)
        {
            var request = await _requestRepository.GetByIdAsync(command.RequestId, cancellationToken);

            if (request == null)
                return Result.Failure(Request.Errors.NotFound);

            if (request.Userid != command.OwnerId)
                return Result.Failure(Request.Errors.Unauthorized);

            if (request.Status == "Approved")
                return Result.Failure(Request.Errors.AlreadyApproved);

            var adoptCommand = new AdoptAnimalCommand
            {
                AnimalId = request.AnimalId,
                AdopterId = request.Useridreq
            };

            var adoptResult = await _mediator.Send(adoptCommand, cancellationToken);

            if (adoptResult.IsFailure)
                return Result.Failure(adoptResult.Error);

            // Update request status
            request.Status = "Approved";
            _requestRepository.Update(request);
            await _requestRepository.SaveChangesAsync();
            await _notificationService.SendNotificationToUserAsync(request.Useridreq, "RequestApproved", new { RequestId = request.Reqid, AnimalId = request.AnimalId });

            // Cache invalidation after write - don't use cancellationToken
            await _cache.RemoveAsync($"requests:user:{request.Useridreq}");

            return Result.Success();
        }
    }
}
