using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Application.Queries;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler
{
    public class GetAnimalByIdQueryHandler : IRequestHandler<GetAnimalByIdQuery, ReadAnimalResponse>
    {
        private readonly IRepository<Animal> _animalRepository;

        public GetAnimalByIdQueryHandler(IRepository<Animal> animalRepository)
        {
            _animalRepository = animalRepository;
        }

        public async Task<ReadAnimalResponse> Handle(GetAnimalByIdQuery request, CancellationToken cancellationToken)
        {
            var animal = await _animalRepository.GetByIdAsync(request.AnimalId);
            if (animal == null)
            {
                throw new KeyNotFoundException($"Animal with ID {request.AnimalId} not found.");
            }

            var response = new ReadAnimalResponse
            {
                AnimalId = animal.AnimalId,
                Name = animal.Name,
                Age = animal.Age,
                Type = animal.Type,
                Breed = animal.Breed,
                Gender = animal.Gender,
                Photo = animal.Photo,
                About = animal.About,
                IsAdopted = animal.IsAdopted,
                Userid = animal.Userid
            };
            return response;
        }
    }
}

