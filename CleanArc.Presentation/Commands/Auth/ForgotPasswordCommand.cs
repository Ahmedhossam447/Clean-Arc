using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Commands.Auth
{
    public class ForgotPasswordCommand : IRequest<Result<ForgotPasswordResponse>>
    {
        public string Email { get; set; } = string.Empty;
    }
}
