using CleanArc.Application.Commands.Auth;
using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using System.Net;

namespace CleanArc.Application.Handlers.CommandsHandler.Auth
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<ConfirmEmailResponse>>
    {
        private readonly IAuthService _authService;

        public ConfirmEmailCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<ConfirmEmailResponse>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var decodedEmail = WebUtility.UrlDecode(request.Email);
            var decodedToken = WebUtility.UrlDecode(request.Token);
            
            var isConfirmed = await _authService.IsEmailConfirmedAsync(decodedEmail);
            if (isConfirmed)
            {
                return EmailErrors.AlreadyConfirmed;
            }

            var result = await _authService.ConfirmEmailAsync(decodedEmail, decodedToken);
            
            if (!result)
            {
                return EmailErrors.ConfirmationFailed;
            }

            return new ConfirmEmailResponse();
        }
    }
}
