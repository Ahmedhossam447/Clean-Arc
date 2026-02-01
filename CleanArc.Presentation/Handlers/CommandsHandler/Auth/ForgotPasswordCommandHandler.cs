using CleanArc.Application;
using CleanArc.Application.Commands.Auth;
using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace CleanArc.Application.Handlers.CommandsHandler.Auth
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result<ForgotPasswordResponse>>
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public ForgotPasswordCommandHandler(
            IAuthService authService,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _authService = authService;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<Result<ForgotPasswordResponse>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var token = await _authService.GeneratePasswordResetTokenAsync(request.Email);
            
            if (string.IsNullOrEmpty(token))
            {
                return PasswordErrors.UserNotFound;
            }

            var encodedToken = WebUtility.UrlEncode(token);
            var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://localhost:7193";
            var resetLink = $"{baseUrl}/api/auth/reset-password?email={WebUtility.UrlEncode(request.Email)}&token={encodedToken}";

            var emailBody = EmailTemplates.GetPasswordResetBody(resetLink);
            var subject = EmailTemplates.GetPasswordResetSubject();

            await _emailService.SendEmailAsync(
                request.Email,
                subject,
                emailBody,
                isHtml: true
            );

            return new ForgotPasswordResponse();
        }
    }
}
