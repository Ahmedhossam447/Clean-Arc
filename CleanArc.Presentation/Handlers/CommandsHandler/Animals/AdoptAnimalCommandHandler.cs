using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Entites;
using CleanArc.Core.Events;
using CleanArc.Core.Interfaces;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Animals
{
    public class AdoptAnimalCommandHandler : IRequestHandler<AdoptAnimalCommand, AdoptAnimalResponse>
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

        public async Task<AdoptAnimalResponse> Handle(AdoptAnimalCommand request, CancellationToken cancellationToken)
        {
            var animal = await _animalRepository.GetByIdAsync(request.AnimalId);
            if (animal == null)
            {
                throw new KeyNotFoundException($"Animal with ID {request.AnimalId} not found");
            }

            if (animal.IsAdopted)
            {
                throw new InvalidOperationException("This animal has already been adopted");
            }

            // Use IUserService instead of UserManager (Clean Architecture compliant)
            var owner = await _userService.GetUserByIdAsync(animal.Userid ?? string.Empty);
            if (owner == null)
            {
                throw new KeyNotFoundException($"Owner with ID {animal.Userid} not found");
            }

            var adopter = await _userService.GetUserByIdAsync(request.AdopterId);
            if (adopter == null)
            {
                throw new KeyNotFoundException($"Adopter with ID {request.AdopterId} not found");
            }

            if (animal.Userid == request.AdopterId)
            {
                throw new InvalidOperationException("You cannot adopt your own animal");
            }

            animal.IsAdopted = true;
            _animalRepository.Update(animal);
            await _animalRepository.SaveChangesAsync();

            // Invalidate the specific animal cache
            try { 

                await _cache.RemoveAsync($"animal:{animal.AnimalId}", cancellationToken);
                await _cache.RemoveAsync($"animals:available:{request.AdopterId}", cancellationToken);
                await _cache.RemoveAsync($"animals:available:{owner.Id}", cancellationToken);
            }
            catch (Exception ex)
            {
                

            }
            // Invalidate both adopter's and owner's available animals cache


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
}
