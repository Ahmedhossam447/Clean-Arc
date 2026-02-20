using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Commands.Auth
{
    public class LoginCommand :IRequest<Result<LoginResponse>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
