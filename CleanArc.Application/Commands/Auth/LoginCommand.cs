using CleanArc.Application.Contracts.Responses.Auth;
using MediatR;

namespace CleanArc.Application.Commands.Auth
{
    public class LoginCommand :IRequest<LoginResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
