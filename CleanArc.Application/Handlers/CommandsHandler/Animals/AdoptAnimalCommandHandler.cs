using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Entities;
using CleanArc.Core.Events;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;

using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace CleanArc.Application.Handlers.CommandsHandler.Animals;

public class AdoptAnimalCommandHandler : IRequestHandler<AdoptAnimalCommand, Result<AdoptAnimalResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventPublisher _eventPublisher;
    private readonly IUserService _userService;
    private readonly IDistributedCache _cache;
    private readonly INotificationService _notificationService;
    private readonly IBackgroundJobService _jobService;
    private readonly ILogger<AdoptAnimalCommandHandler> _logger;

    public AdoptAnimalCommandHandler(
        IUnitOfWork unitOfWork,
        IEventPublisher eventPublisher,
        IUserService userService,
        IDistributedCache cache,
        IBackgroundJobService jobService,
        INotificationService notificationService,
        ILogger<AdoptAnimalCommandHandler> logger)
    {
        _eventPublisher = eventPublisher;
        _userService = userService;
        _cache = cache;
        _unitOfWork = unitOfWork;
        _jobService = jobService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result<AdoptAnimalResponse>> Handle(
        AdoptAnimalCommand request,
        CancellationToken cancellationToken)
    {
        var animal = await _unitOfWork.AnimalRepository.GetByIdAsync(request.AnimalId, cancellationToken);
        if (animal == null)
            return Animal.Errors.NotFound;

        var owner = await _userService.GetUserByIdAsync(animal.OwnerId ?? string.Empty, cancellationToken);
        if (owner == null)
            return UserErrors.OwnerNotFound;

        var adopter = await _userService.GetUserByIdAsync(request.AdopterId, cancellationToken);
        if (adopter == null)
            return UserErrors.AdopterNotFound;

        var acceptedRequest = (await _unitOfWork.Repository<Request>().GetAsync(
            a => a.AnimalId == request.AnimalId && a.RequesterId == request.AdopterId, 
            cancellationToken)).FirstOrDefault();
        if (acceptedRequest == null)
            return Request.Errors.NotFound;

        var adoptResult = animal.Adopt(adopter.Id);
        if (adoptResult.IsFailure)
            return adoptResult.Error;

        // Begin transaction for atomic operations
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _unitOfWork.Repository<Notification>().AddAsync(new Notification
            {
                UserId = request.AdopterId,
                Message = $"?? Congratulations! You are now the official owner of {animal.Name ?? "this animal"}!",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            });

            // Commit transaction - saves animal adoption and notification atomically
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        // Send real-time notification (non-critical, can fail without breaking adoption)
        try
        {
            await _notificationService.SendNotificationToUserAsync(
                request.AdopterId,
                "ReceiveNotification",
                $"?? Congratulations! You are the new owner of {animal.Name ?? "this animal"}!"
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send SignalR notification to adopter {AdopterId} for animal {AnimalId}", 
                request.AdopterId, request.AnimalId);
        }

        // Enqueue background job to process rejected requests
        _jobService.EnqueueJob<IAdoptionBackgroundService>(x =>
            x.ProcessRejectedRequestsAsync(request.AnimalId, acceptedRequest.Id));

        // Cache invalidation (non-critical, log but don't fail)
        try
        {
            await _cache.RemoveAsync($"animal:{animal.Id}", cancellationToken);
            await _cache.RemoveAsync($"animals:available:{request.AdopterId}", cancellationToken);
            await _cache.RemoveAsync($"animals:available:{owner.Id}", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to invalidate cache for animal {AnimalId}, adopter {AdopterId}, owner {OwnerId}. " +
                "Cache will be stale but adoption was successful.", 
                animal.Id, request.AdopterId, owner.Id);
        }

        // Publish domain event (non-critical, can fail without breaking adoption)
        try
        {
            await _eventPublisher.PublishAsync(new AnimalAdoptedEvent
            {
                AnimalId = animal.Id,
                AdopterId = adopter.Id,
                OwnerId = owner.Id,
                AdoptedAt = DateTime.UtcNow,
                OwnerEmail = owner.Email,
                AdopterEmail = adopter.Email,
                AnimalName = animal.Name ?? "Unknown",
                AnimalType = animal.Type ?? "Unknown",
                AdopterName = adopter.FullName ?? adopter.UserName
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to publish AnimalAdoptedEvent for animal {AnimalId}", animal.Id);
        }

        return new AdoptAnimalResponse
        {
            Succeeded = true,
            animalId = animal.Id
        };
    }
}