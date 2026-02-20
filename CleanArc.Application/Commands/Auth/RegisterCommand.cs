using CleanArc.Core.Primitives;
using CleanArc.Application.Contracts.Responses.Auth;
using MediatR;

namespace CleanArc.Application.Commands.Auth
{
    public class RegisterCommand : IRequest<Result<RegisterResponse>>
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Location { get; set; }
        public string? Bio { get; set; }
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = "User";
    }
}
