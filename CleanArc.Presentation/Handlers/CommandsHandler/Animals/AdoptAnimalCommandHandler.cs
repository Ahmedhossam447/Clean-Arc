using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Entites;
using CleanArc.Core.Entities;
using CleanArc.Core.Events;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Animals;

public class AdoptAnimalCommandHandler : IRequestHandler<AdoptAnimalCommand, Result<AdoptAnimalResponse>>
{
    private readonly IUnitOfWork _Uow;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IUserService _userService;
    private readonly IDistributedCache _cache;
    private readonly INotificationService _notificationService;
    private readonly IBackgroundJobService _jobService;

    public AdoptAnimalCommandHandler(
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint,
        IUserService userService,
        IDistributedCache cache,
        IBackgroundJobService jobService,
        INotificationService notificationService)
    {
        _publishEndpoint = publishEndpoint;
        _userService = userService;
        _cache = cache;
        _Uow = unitOfWork;
        _jobService = jobService;
        _notificationService = notificationService;
    }

    public async Task<Result<AdoptAnimalResponse>> Handle(
        AdoptAnimalCommand request,
        CancellationToken cancellationToken)
    {
 
        var animal = await _Uow.AnimalRepository.GetByIdAsync(request.AnimalId, cancellationToken);
        if (animal == null)
            return Animal.Errors.NotFound;

        var owner = await _userService.GetUserByIdAsync(animal.Userid ?? string.Empty, cancellationToken);
        if (owner == null)
            return UserErrors.OwnerNotFound;

        var adopter = await _userService.GetUserByIdAsync(request.AdopterId, cancellationToken);
        if (adopter == null)
            return UserErrors.AdopterNotFound;
        var AcceptedRequest = (await _Uow.RequestRepository.GetAsync(a => a.AnimalId == request.AnimalId && a.Useridreq == request.AdopterId, cancellationToken)).FirstOrDefault();
        if (AcceptedRequest == null)
            return Request.Errors.NotFound;

        var adoptResult = animal.Adopt(adopter.Id);
        if (adoptResult.IsFailure)
            return adoptResult.Error;
        await _Uow.Repository<Notification>().AddAsync(new Notification
        {
            UserId = request.AdopterId,
            Message = $"🎉 Congratulations! You are now the official owner of {animal.Name}!",
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        });
        await _Uow.SaveChangesAsync();
        await _notificationService.SendNotificationToUserAsync(
            request.AdopterId,
            "ReceiveNotification",
            $"🎉 Congratulations! You are the new owner of {animal.Name}!"
        );
        _jobService.EnqueueJob<IAdoptionBackgroundService>(x =>
        x.ProcessRejectedRequestsAsync(request.AnimalId,AcceptedRequest.Reqid));


        try
        {
            await _cache.RemoveAsync($"animal:{animal.AnimalId}");
            await _cache.RemoveAsync($"animals:available:{request.AdopterId}");
            await _cache.RemoveAsync($"animals:available:{owner.Id}");
        }
        catch
        {
        }

        await _publishEndpoint.Publish(new AnimalAdoptedEvent
        {
            AnimalId = animal.AnimalId,
            AdopterId = adopter.Id,
            OwnerId = owner.Id,
            AdoptedAt = DateTime.UtcNow,
            OwnerEmail = owner.Email,
            AdopterEmail = adopter.Email,
            AnimalName = animal.Name,
            AnimalType = animal.Type,
            AdopterName = adopter.FullName ?? adopter.UserName
        }, cancellationToken);

        return new AdoptAnimalResponse
        {
            Succeeded = true,
            animalId = animal.AnimalId
        };
    }
}
