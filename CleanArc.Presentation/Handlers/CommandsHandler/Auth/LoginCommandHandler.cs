using CleanArc.Application.Commands.Auth;
using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.CommandsHandler.Auth
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(IAuthService authService, ITokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var authUser = await _authService.LoginUserAsync(request.Email, request.Password);
            
            if (authUser == null)
            {
                return new LoginResponse
                {
                    Succeeded = false,
                    Errors = new List<string> { UserErrors.InvalidCredentials.Description }
                };
            }
            var emailconfirmed = await _authService.IsEmailConfirmedAsync(authUser.Email);
            if (!emailconfirmed)
            {
                return new LoginResponse
                {
                    Succeeded = false,
                    Errors = new List<string> { EmailErrors.NotConfirmed.Description }
                };
            }

            var token = await _tokenService.GenerateRefreshTokenAsync(authUser.Id);
            var accessToken = _tokenService.GenerateAccessToken(authUser);

            return new LoginResponse
            {
                Succeeded = true,
                RefreshToken = token.Token,
                RefreshTokenExpiry = token.ExpiresAt,
                AccessToken = accessToken,
                Errors = new List<string>()

            };
        }
    }
}
