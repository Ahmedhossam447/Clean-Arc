using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Entites;
using CleanArc.Core.Events;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Animals;

public class AdoptAnimalCommandHandler : IRequestHandler<AdoptAnimalCommand, Result<AdoptAnimalResponse>>
{
    private readonly IRepository<Animal> _animalRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IUserService _userService;
    private readonly IDistributedCache _cache;

    public AdoptAnimalCommandHandler(
        IRepository<Animal> animalRepository,
        IPublishEndpoint publishEndpoint,
        IUserService userService,
        IDistributedCache cache)
    {
        _publishEndpoint = publishEndpoint;
        _animalRepository = animalRepository;
        _userService = userService;
        _cache = cache;
    }

    public async Task<Result<AdoptAnimalResponse>> Handle(
        AdoptAnimalCommand request,
        CancellationToken cancellationToken)
    {
        var animal = await _animalRepository.GetByIdAsync(request.AnimalId, cancellationToken);
        if (animal == null)
            return Animal.Errors.NotFound;

        var owner = await _userService.GetUserByIdAsync(animal.Userid ?? string.Empty, cancellationToken);
        if (owner == null)
            return UserErrors.OwnerNotFound;

        var adopter = await _userService.GetUserByIdAsync(request.AdopterId, cancellationToken);
        if (adopter == null)
            return UserErrors.AdopterNotFound;

        var adoptResult = animal.Adopt(adopter.Id);
        if (adoptResult.IsFailure)
            return adoptResult.Error;

        await _animalRepository.SaveChangesAsync();

       
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
