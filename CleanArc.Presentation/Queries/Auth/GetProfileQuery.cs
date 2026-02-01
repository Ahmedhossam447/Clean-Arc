using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Queries.Auth
{
    public class GetProfileQuery : IRequest<Result<ProfileResponse>>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
