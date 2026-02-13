using CleanArc.Application.Commands.Auth;
using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.CommandsHandler.Auth
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<LogoutResponse>>
    {
        private readonly ITokenService _tokenService;

        public LogoutCommandHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }
        public async Task<Result<LogoutResponse>> Handle(LogoutCommand request, CancellationToken cancellationToken) { 
        
          var Result =await _tokenService.RevokeRefreshTokenAsync(request.RefreshToken);
            if (Result.IsFailure) return Result.Error;

            return new LogoutResponse
            {
                Succeeded = true
            };
        }
    }
}

