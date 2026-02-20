using CleanArc.Application.Commands.Auth;
using CleanArc.Application.Contracts.Responses.Auth;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Handlers.CommandsHandler.Auth
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, Result<GoogleLoginResponse>>
    {
        private readonly ITokenService _tokenService;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        
        public GoogleLoginCommandHandler(ITokenService tokenService, IGoogleAuthService googleAuthService, IUserService userService, IAuthService authService)
        {
            _tokenService = tokenService;
            _googleAuthService = googleAuthService;
            _userService = userService;
            _authService = authService;
        }
        public async Task<Result<GoogleLoginResponse>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            var payload =await _googleAuthService.ValidateTokenAsync(request.TokenId);
            if (payload == null)
            {
                return payload.Error;
            }
            var user = await _userService.GetUserByEmailAsync(payload.Value.Email);
            if (user == null) { 
                var baseUsername = payload.Value.Email.Split('@')[0];
                var randomSuffix = Guid.NewGuid().ToString("N").Substring(0, 4);
                var generatedUsername = $"{baseUsername}_{randomSuffix}";
                var generatedPassword = Guid.NewGuid().ToString("N") + "aA1!"; // Meet complexity requirements

                var registerResult = await _authService.RegisterUserAsync(generatedUsername, generatedPassword, payload.Value.Email, true, "User", payload.Value.Name);
                if (!registerResult.Succeeded)
                {
                    user = await _userService.GetUserByEmailAsync(payload.Value.Email);
                    if (user == null) 
                    {
                        return Result<GoogleLoginResponse>.Failure(UserErrors.GoogleRegistrationFailed(string.Join(" ", registerResult.Errors)));
                    }
                }
                else 
                {
                    user = await _userService.GetUserByEmailAsync(payload.Value.Email);
                    if (user == null)
                        return Result<GoogleLoginResponse>.Failure(UserErrors.GoogleUserRetrievalFailed);
                }
            }
            var emailConfirmed = await _authService.IsEmailConfirmedAsync(user.Email, cancellationToken);
            if (!emailConfirmed)
            {
                await _authService.VerifyEmailForGoogleAuthAsync(user.Email);
            }
            await _authService.AddExternalLoginAsync(user.Email, "Google", payload.Value.Subject);
            var AccessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken =await _tokenService.GenerateRefreshTokenAsync(user.Id);
            return new GoogleLoginResponse
            {
                Email = user.Email,
                Name = user.UserName,
                AccessToken = AccessToken,
                RefreshToken = refreshToken.Token
            };
        }
    }
}
