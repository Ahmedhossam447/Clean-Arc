using CleanArc.Application.Contracts.Responses.Token;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Commands.Token
{
    public class RefreshTokenCommand : IRequest<Result<RefreshTokenResponse>>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
