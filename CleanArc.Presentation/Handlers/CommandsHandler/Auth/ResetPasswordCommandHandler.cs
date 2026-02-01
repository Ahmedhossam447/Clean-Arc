using CleanArc.Application.Commands.Auth;
using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using System.Net;

namespace CleanArc.Application.Handlers.CommandsHandler.Auth
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<ResetPasswordResponse>>
    {
        private readonly IAuthService _authService;

        public ResetPasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<ResetPasswordResponse>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var decodedEmail = WebUtility.UrlDecode(request.Email);
            var decodedToken = WebUtility.UrlDecode(request.Token);
            
            var result = await _authService.ResetPasswordAsync(decodedEmail, decodedToken, request.NewPassword);
            
            if (!result)
            {
                return PasswordErrors.ResetFailed;
            }

            return new ResetPasswordResponse();
        }
    }
}
