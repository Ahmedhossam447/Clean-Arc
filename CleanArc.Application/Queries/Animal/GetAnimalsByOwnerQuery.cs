using CleanArc.Application.Contracts.Responses;
using CleanArc.Application.Contracts.Responses.Animal;
using MediatR;

namespace CleanArc.Application.Queries.Animal;

public class GetAnimalsByOwnerQuery : IRequest<PaginationResponse<ReadAnimalResponse>>
{
    public string OwnerId { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
