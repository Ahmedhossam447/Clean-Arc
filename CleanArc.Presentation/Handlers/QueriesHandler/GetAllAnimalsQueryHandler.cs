using CleanArc.Application.Contracts.Requests;
using CleanArc.Application.Contracts.Responses;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Application.Queries.Animal;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;
using System.Linq;

namespace CleanArc.Application.Handlers.QueriesHandler
{
    public class GetAllAnimalsQueryHandler : IRequestHandler<GetAllAnimalsQuery,PaginationResponse<ReadAnimalResponse>>
    {
        private readonly IRepository<Animal> _repository;
        public GetAllAnimalsQueryHandler(IRepository<Animal> repository)
        {
            _repository = repository;
        }
        public async Task<PaginationResponse<ReadAnimalResponse>> Handle(GetAllAnimalsQuery request, CancellationToken cancellationToken)
        {
            var animals = await _repository.GetAllAsync();
            int totalCount = animals.Count();
            int TotalPages = (int)Math.Ceiling((double)animals.Count() / request.PageSize);
            animals = animals
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var allAnimalsResponse = new GetAllAnimalsResponse
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
            var response = new PaginationResponse<ReadAnimalResponse>
            {
                Items = allAnimalsResponse.Animals,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = totalCount,
                TotalPages = TotalPages
            };

            return response;

        }
    }
}
