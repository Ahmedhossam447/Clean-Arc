using CleanArc.Application.Commands.Auth;
using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.CommandsHandler.Auth
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(IAuthService authService, ITokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }

        public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var authUser = await _authService.LoginUserAsync(request.Email, request.Password, cancellationToken);
            
            if (authUser == null)
            {
                return Result<LoginResponse>.Failure(UserErrors.InvalidCredentials);
            }
            var emailconfirmed = await _authService.IsEmailConfirmedAsync(authUser.Email, cancellationToken);
            if (!emailconfirmed)
            {
                return Result<LoginResponse>.Failure(EmailErrors.NotConfirmed);
            }

            var token = await _tokenService.GenerateRefreshTokenAsync(authUser.Id);
            var accessToken = _tokenService.GenerateAccessToken(authUser);

            return new LoginResponse
            {
                RefreshToken = token.Token,
                RefreshTokenExpiry = token.ExpiresAt,
                AccessToken = accessToken,
            };
        }
    }
}
