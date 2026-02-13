using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Queries.Auth
{
    public class GetUserProfileQuery : IRequest<Result<PublicProfileResponse>>, ICacheableQuery
    {
        [JsonIgnore]
        public string UserId { get; set; } = string.Empty;

        public string CacheKey => $"user:profile:{UserId}";
    }
}
