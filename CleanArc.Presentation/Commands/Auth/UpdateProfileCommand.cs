using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Commands.Auth
{
    public class UpdateProfileCommand : IRequest<Result<ProfileResponse>>
    {
        public string UserId { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Location { get; set; }
        public string? Bio { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
