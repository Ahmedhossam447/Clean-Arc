using CleanArc.Application.Commands.Auth;
using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Interfaces;
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
                    Errors = new List<string> { "Invalid email or password." }
                };
            }

            // AuthUser is now a clean Core model - no EF/Identity dependency
            var token = _tokenService.GenerateAccessToken(authUser);
            
            return new LoginResponse
            {
                Succeeded = true,
                Token = token
            };
        }
    }
}
