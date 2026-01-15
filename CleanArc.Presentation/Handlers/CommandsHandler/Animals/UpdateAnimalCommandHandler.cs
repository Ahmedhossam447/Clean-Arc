using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Handlers.CommandsHandler.Animals
{
    public class UpdateAnimalCommandHandler : IRequestHandler<UpdateAnimalCommand, UpdateAnimalResponse>
    {
        private readonly IRepository<Animal> _animalRepository;
        public UpdateAnimalCommandHandler(IRepository<Animal> animalRepository)
        {
            _animalRepository = animalRepository;
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
