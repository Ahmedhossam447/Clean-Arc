using CleanArc.Application.Contracts.Responses;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Application.Queries.Animal;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.Animals;

public class GetAvailableAnimalsForAdoptionHandler : IRequestHandler<GetAvailableAnimalsForAdoptionQuery, PaginationResponse<ReadAnimalResponse>>
{
    private readonly IAnimalRepository _animalRepository;

    public GetAvailableAnimalsForAdoptionHandler(IAnimalRepository animalRepository)
    {
        _animalRepository = animalRepository;
    }

    public async Task<PaginationResponse<ReadAnimalResponse>> Handle(GetAvailableAnimalsForAdoptionQuery request, CancellationToken cancellationToken)
    {
        var animals = await _animalRepository.GetAvailableAnimalsForAdoption(request.UserId);

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
                Age = a.Age,
                Breed = a.Breed,
                Photo = a.Photo,
                About = a.About,
                Gender = a.Gender,
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
