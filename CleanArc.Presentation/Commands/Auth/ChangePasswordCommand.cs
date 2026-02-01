using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Commands.Auth
{
    public class ChangePasswordCommand : IRequest<Result<ChangePasswordResponse>>
    {
        public string UserId { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
