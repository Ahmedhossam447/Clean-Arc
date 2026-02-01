using CleanArc.Application.Contracts.Responses;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Application.Queries.Animal;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.Animals;

public class GetAnimalByOwnerQueryHandler : IRequestHandler<GetAnimalsByOwnerQuery, PaginationResponse<ReadAnimalResponse>>
{
    private readonly IRepository<Animal> _animalRepository;

    public GetAnimalByOwnerQueryHandler(IRepository<Animal> animalRepository)
    {
        _animalRepository = animalRepository;
    }

    public async Task<PaginationResponse<ReadAnimalResponse>> Handle(GetAnimalsByOwnerQuery request, CancellationToken cancellationToken)
    {
        var animals = await _animalRepository.GetAsync(a => a.Userid == request.OwnerId, cancellationToken);

        int totalCount = animals.Count();
        int totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var pagedAnimals = animals
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new ReadAnimalResponse
            {
                AnimalId = a.AnimalId,
                Name = a.Name,
                Type = a.Type,
                Breed = a.Breed,
                Age = a.Age,
                Gender = a.Gender,
                Photo = a.Photo,
                About = a.About,
                IsAdopted = a.IsAdopted,
                Userid = a.Userid
            }).ToList();

        return new PaginationResponse<ReadAnimalResponse>
        {
            Items = pagedAnimals,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalItems = totalCount,
            TotalPages = totalPages
        };
    }
}
