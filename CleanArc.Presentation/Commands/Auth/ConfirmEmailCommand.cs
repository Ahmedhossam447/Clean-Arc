using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Commands.Auth
{
    public class ConfirmEmailCommand : IRequest<Result<ConfirmEmailResponse>>
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
