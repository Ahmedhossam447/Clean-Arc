using CleanArc.Application.Commands.Animal;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Handlers.CommandsHandler.Animals
{
    public class CreateAnimalCommandHandler : IRequestHandler<CreateAnimalCommand, CreateAnimalResponse>
    {
        private readonly IRepository<Core.Entites.Animal> _animalRepository;
        public CreateAnimalCommandHandler(IRepository<Core.Entites.Animal> animalRepository)
        {
            _animalRepository = animalRepository;
        }
        public async Task<CreateAnimalResponse> Handle(CreateAnimalCommand request, CancellationToken cancellationToken)
        {
            var animal = new Core.Entites.Animal
            {
                Name = request.Name,
                Age = request.Age,
                Type = request.Type,
                Breed = request.Breed,
                About = request.About,
                IsAdopted = false,
                Userid = request.Userid,
                Gender = request.Gender,
                Photo = request.Photo,
            };
            var animalResponse=await _animalRepository.AddAsync(animal);
            await _animalRepository.SaveChangesAsync();
            var response = new CreateAnimalResponse
            {
                AnimalId = animalResponse.AnimalId,
                Name = animalResponse.Name,
                Age = animalResponse.Age,
                Type = animalResponse.Type,
                Breed = animalResponse.Breed,
                About = animalResponse.About,
                Userid = animalResponse.Userid,
                Gender =animalResponse.Gender,
                Photo = animalResponse.Photo
            };
            return response;

        }
    }
}
