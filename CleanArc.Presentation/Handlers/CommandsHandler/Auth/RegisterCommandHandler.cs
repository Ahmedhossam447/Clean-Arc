using CleanArc.Application;
using CleanArc.Application.Commands.Auth;
using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Interfaces;
using MediatR;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace CleanArc.Application.Handlers.CommandsHandler.Auth
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public RegisterCommandHandler(
            IAuthService authService,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _authService = authService;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterUserAsync(
                request.Username, 
                request.Password, 
                request.Email,
                request.Role,
                request.FullName,
                request.PhotoUrl,
                request.Location,
                request.Bio,
                request.PhoneNumber);
            
            if (result.Succeeded)
            {
                var token = await _authService.GenerateEmailConfirmationTokenAsync(request.Email);
                
                if (!string.IsNullOrEmpty(token))
                {
                    var encodedToken = WebUtility.UrlEncode(token);
                    var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://localhost:7193";
                    var confirmationLink = $"{baseUrl}/api/auth/confirm-email?email={WebUtility.UrlEncode(request.Email)}&token={encodedToken}";

                    var emailBody = EmailTemplates.GetEmailConfirmationBody(confirmationLink, request.Username);
                    var subject = EmailTemplates.GetEmailConfirmationSubject();

                    await _emailService.SendEmailAsync(
                        request.Email,
                        subject,
                        emailBody,
                        isHtml: true
                    );
                }

                return new RegisterResponse
                {
                    Succeeded = true,
                    Errors = Array.Empty<string>()
                };
            }
            else
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
