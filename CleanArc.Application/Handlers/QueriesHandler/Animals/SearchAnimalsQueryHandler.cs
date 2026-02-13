using CleanArc.Application.Contracts.Responses;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Application.Queries.Animal;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.Animals;

public class SearchAnimalsQueryHandler : IRequestHandler<SearchAnimalsQuery, PaginationResponse<ReadAnimalResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchAnimalsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginationResponse<ReadAnimalResponse>> Handle(SearchAnimalsQuery request, CancellationToken cancellationToken)
    {
        var animals = await _unitOfWork.Repository<Animal>().GetAsync(a =>
            (string.IsNullOrEmpty(request.Type) || a.Type == request.Type) &&
            (string.IsNullOrEmpty(request.Breed) || a.Breed == request.Breed) &&
            (string.IsNullOrEmpty(request.Gender) || a.Gender == request.Gender) &&
            !a.IsAdopted, cancellationToken);

        int totalCount = animals.Count();
        int totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var pagedAnimals = animals
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new ReadAnimalResponse
            {
                AnimalId = a.Id,
                Name = a.Name,
                Type = a.Type,
                Breed = a.Breed,
                Age = a.Age,
                Gender = a.Gender,
                Photo = a.Photo,
                About = a.About,
                IsAdopted = a.IsAdopted,
                OwnerId = a.OwnerId
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
