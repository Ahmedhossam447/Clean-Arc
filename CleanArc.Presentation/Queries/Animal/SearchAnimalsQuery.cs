using CleanArc.Application.Contracts.Responses;
using CleanArc.Application.Contracts.Responses.Animal;
using MediatR;

namespace CleanArc.Application.Queries.Animal;

public class SearchAnimalsQuery : IRequest<PaginationResponse<ReadAnimalResponse>>
{
    public string? Type { get; set; }
    public string? Breed { get; set; }
    public string? Gender { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
