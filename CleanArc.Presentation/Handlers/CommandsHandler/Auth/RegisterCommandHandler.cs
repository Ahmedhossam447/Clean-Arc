using CleanArc.Application.Commands.Auth;
using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Interfaces;
using MediatR;


namespace CleanArc.Application.Handlers.CommandsHandler.Auth
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
    {
        private readonly IAuthService _authService;
        public RegisterCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterUserAsync(request.Username, request.Password, request.Email);
            if (result.Succeeded)
            {
                return new RegisterResponse
                {
                    Succeeded = true,
                    Errors = Array.Empty<string>()
                };
            }else
                            {
                return new RegisterResponse
                {
                    Succeeded = false,
                    Errors = result.Errors
                };
            }
    }
}
}
