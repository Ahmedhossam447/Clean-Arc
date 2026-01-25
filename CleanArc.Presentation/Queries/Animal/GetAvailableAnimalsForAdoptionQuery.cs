using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Queries.Animal
{
    public class GetAvailableAnimalsForAdoptionQuery : IRequest<GetAvailableAnimalsForAdoptionResponse>, ICacheableQuery
    {
        public string UserId { get; set; } = string.Empty;

        // Cache key includes UserId because each user sees a different list
        // (they can't adopt their own animals)
        public string CacheKey => $"animals:available:{UserId}";

        // Shorter TTL for lists - they change more frequently
        public TimeSpan? CacheDuration => TimeSpan.FromMinutes(2);
    }
}
