using CleanArc.Application.Commands.Token;
using CleanArc.Application.Contracts.Responses.Token;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.CommandsHandler.Token
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
    {
        private readonly ITokenService _tokenService;

        public RefreshTokenCommandHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var result = await _tokenService.RefreshTokensAsync(request.RefreshToken);

            if (result.IsFailure)
                return result.Error;

            return new RefreshTokenResponse
            {
                AccessToken = result.Value.AccessToken,
                RefreshToken = result.Value.NewRefreshToken.Token
            };
        }
    }
}
