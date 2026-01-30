using CleanArc.Application.Contracts.Responses;
using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Queries.Animal;

public class GetAvailableAnimalsForAdoptionQuery : IRequest<PaginationResponse<ReadAnimalResponse>>, ICacheableQuery
{
    public string UserId { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string CacheKey => $"animals:available:{UserId}:page:{PageNumber}";
    public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
}
