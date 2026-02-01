using CleanArc.Application.Commands.Auth;
using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CleanArc.Application.Handlers.CommandsHandler.Auth
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<ConfirmEmailResponse>>
    {
        private readonly IAuthService _authService;
        private readonly ILogger<ConfirmEmailCommandHandler> _logger;

        public ConfirmEmailCommandHandler(
            IAuthService authService,
            ILogger<ConfirmEmailCommandHandler> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public async Task<Result<ConfirmEmailResponse>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            // Handle empty or null email/token
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Token))
            {
                _logger.LogWarning("Email confirmation attempted with empty email or token");
                return EmailErrors.InvalidToken;
            }

            // ASP.NET Core automatically URL-decodes [FromQuery] parameters
            // Use the values directly - they should already be decoded
            var email = request.Email ?? string.Empty;
            var token = request.Token ?? string.Empty;
            
            // Log the raw token length for debugging (don't log the actual token for security)
            _logger.LogInformation("Token length: {TokenLength}, Email: {Email}", token.Length, email);
            
            _logger.LogInformation("Attempting email confirmation for: {Email}", email);
            
            var isConfirmed = await _authService.IsEmailConfirmedAsync(email, cancellationToken);
            if (isConfirmed)
            {
                _logger.LogInformation("Email already confirmed: {Email}", email);
                return EmailErrors.AlreadyConfirmed;
            }

            var result = await _authService.ConfirmEmailAsync(email, token);
            
            if (!result.Succeeded)
            {
                _logger.LogWarning("Email confirmation failed for {Email}. Error: {Error}", email, result.ErrorMessage);
                
                // Return more specific error if available
                if (result.ErrorMessage?.Contains("Invalid", StringComparison.OrdinalIgnoreCase) == true || 
                    result.ErrorMessage?.Contains("expired", StringComparison.OrdinalIgnoreCase) == true ||
                    result.ErrorMessage?.Contains("token", StringComparison.OrdinalIgnoreCase) == true)
                {
                    return EmailErrors.InvalidToken;
                }
                return EmailErrors.ConfirmationFailed;
            }

            _logger.LogInformation("Email confirmed successfully for: {Email}", email);
            return new ConfirmEmailResponse();
        }
    }
}
