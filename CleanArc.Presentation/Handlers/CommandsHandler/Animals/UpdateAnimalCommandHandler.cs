using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace CleanArc.Application.Handlers.CommandsHandler.Animals
{
    public class UpdateAnimalCommandHandler : IRequestHandler<UpdateAnimalCommand, UpdateAnimalResponse>
    {
        private readonly IRepository<Animal> _animalRepository;
        private readonly IDistributedCache _cache;

        public UpdateAnimalCommandHandler(IRepository<Animal> animalRepository, IDistributedCache cache)
        {
            _animalRepository = animalRepository;
            _cache = cache;
        }

        public async Task<UpdateAnimalResponse> Handle(UpdateAnimalCommand request, CancellationToken cancellationToken)
        {
            var animal = await _animalRepository.GetByIdAsync(request.AnimalId);
            if (animal == null)
            {
                throw new KeyNotFoundException($"Animal with ID {request.AnimalId} not found");
            }
            animal.Name = request.Name ?? animal.Name;
            animal.Age = request.Age ?? animal.Age;
            animal.Type = request.Type ?? animal.Type;
            animal.Breed = request.Breed ?? animal.Breed;
            animal.About = request.About ?? animal.About;
            animal.Photo = request.Photo ?? animal.Photo;
            _animalRepository.Update(animal);
            await _animalRepository.SaveChangesAsync();

            // Invalidate the specific animal cache
            await _cache.RemoveAsync($"animal:{animal.AnimalId}", cancellationToken);
            // Invalidate the owner's available animals cache
            await _cache.RemoveAsync($"animals:available:{animal.Userid}", cancellationToken);

            var response = new UpdateAnimalResponse
            {
                AnimalId = animal.AnimalId,
                Name = animal.Name,
                Age = animal.Age,
                Type = animal.Type,
                Breed = animal.Breed,
                About = animal.About,
                Userid = animal.Userid,
                Gender = animal.Gender,
                Photo = animal.Photo
            };
            return response;
        }
    }
}
