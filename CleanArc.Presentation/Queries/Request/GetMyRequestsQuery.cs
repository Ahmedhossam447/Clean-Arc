using CleanArc.Application.Contracts.Responses.Request;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Queries.Request
{
    public class GetMyRequestsQuery : IRequest<List<RequestResponse>>, ICacheableQuery
    {
        public string UserId { get; set; } = string.Empty;

        public string CacheKey => $"requests:user:{UserId}";
        public TimeSpan? CacheDuration => TimeSpan.FromMinutes(10);
    }
}
