using CleanArc.Application.Commands;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Handlers.CommandsHandler
{
    public class CreateAnimalCommandHandler : IRequestHandler<CreateAnimalCommand, CreateAnimalResponse>
    {
        private readonly IRepository<Animal> _animalRepository;
        public CreateAnimalCommandHandler(IRepository<Animal> animalRepository)
        {
            _animalRepository = animalRepository;
        }
        public async Task<CreateAnimalResponse> Handle(CreateAnimalCommand request, CancellationToken cancellationToken)
        {
            var animal = new Animal
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
