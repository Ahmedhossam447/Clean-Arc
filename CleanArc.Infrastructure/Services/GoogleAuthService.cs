using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CleanArc.Infrastructure.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _configuration;
        
        public GoogleAuthService (IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<Result<GoogleUser>> ValidateTokenAsync(string TokenId, CancellationToken cancellationToken = default)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { _configuration["GoogleAuth:ClientId"] }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(TokenId, settings);

                return Result<GoogleUser>.Success(new GoogleUser(payload.Email, payload.Name, payload.Subject));

            }
            catch (InvalidJwtException)
            {
                return Result<GoogleUser>.Failure(new Error("GoogleAuth.InvalidToken", "The provided Google token is invalid."));
            }
            catch (Exception ex)
            {
                return Result<GoogleUser>.Failure(new Error("GoogleAuth.ValidationError", "An error occurred while validating the Google token."));
            }
        }
    }
}
