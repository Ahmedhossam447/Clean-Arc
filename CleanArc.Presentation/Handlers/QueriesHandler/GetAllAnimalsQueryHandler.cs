using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Application.Queries.Animal;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;
using System.Linq;

namespace CleanArc.Application.Handlers.QueriesHandler
{
    public class GetAllAnimalsQueryHandler : IRequestHandler<GetAllAnimalsQuery, GetAllAnimalsResponse>
    {
        private readonly IRepository<Animal> _repository;
        public GetAllAnimalsQueryHandler(IRepository<Animal> repository)
        {
            _repository = repository;
        }
        public async Task<GetAllAnimalsResponse> Handle(GetAllAnimalsQuery request, CancellationToken cancellationToken)
        {
            var animals = await _repository.GetAllAsync();
            var response = new GetAllAnimalsResponse
            {
                Animals = animals.Select(a => new ReadAnimalResponse
                {
                    AnimalId = a.AnimalId,
                    Name = a.Name,
                    Type = a.Type,
                    Age = a.Age,
                    Breed = a.Breed,
                    Photo = a.Photo,
                    About = a.About,
                    Gender = a.Gender,
                    IsAdopted = a.IsAdopted,
                    Userid = a.Userid
                }).ToList()
            };
            return response;

        }
    }
}
