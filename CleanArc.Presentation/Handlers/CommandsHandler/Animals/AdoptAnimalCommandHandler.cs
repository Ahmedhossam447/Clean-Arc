using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Entites;
using CleanArc.Core.Events;
using CleanArc.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Animals
{
    public class AdoptAnimalCommandHandler : IRequestHandler<AdoptAnimalCommand, AdoptAnimalResponse>
    {
        private readonly IRepository<Animal> _animalRepository;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDistributedCache _cache;

        public AdoptAnimalCommandHandler(
            IRepository<Animal> animalRepository,
            IMediator mediator,
            UserManager<ApplicationUser> userManager,
            IDistributedCache cache)
        {
            _animalRepository = animalRepository;
            _mediator = mediator;
            _userManager = userManager;
            _cache = cache;
        }
        public async Task<AdoptAnimalResponse> Handle(AdoptAnimalCommand request, CancellationToken cancellationToken)
        {
            var animal =await  _animalRepository.GetByIdAsync(request.AnimalId);
            if (animal == null)
            {
                throw new KeyNotFoundException($"Animal with ID {request.AnimalId} not found");
            }
            if (animal.IsAdopted)
            {
                throw new InvalidOperationException("This animal has already been adopted");
            }
            var owner = await _userManager.FindByIdAsync(animal.Userid);
            if (owner == null)
            {
                throw new KeyNotFoundException($"Owner with ID {animal.Userid} not found");
            }
            var adopter = await _userManager.FindByIdAsync(request.AdopterId);
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
            await _cache.RemoveAsync($"animal:{animal.AnimalId}", cancellationToken);
            // Invalidate both adopter's and owner's available animals cache
            await _cache.RemoveAsync($"animals:available:{request.AdopterId}", cancellationToken);
            await _cache.RemoveAsync($"animals:available:{owner.Id}", cancellationToken);

            // Raise AnimalAdoptedEvent
            await _mediator.Publish(new AnimalAdoptedEvent(
            
                animal.AnimalId,
                animal.Name,
                animal.Type,
                adopter.Id,
                adopter.UserName,
                 adopter.Email,
                owner.Id,
                owner.Email
            ), cancellationToken);
            return new AdoptAnimalResponse
            {
                Succeeded = true,
                animalId = animal.AnimalId
            };
        }
    }
}
