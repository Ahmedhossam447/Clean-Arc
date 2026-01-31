using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Models.Identity;
using CleanArc.Core.Primitives;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CleanArc.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<RefreshToken> _refreshTokenRepository;
        private readonly IUserService _userService;

        public TokenService(
            IConfiguration configuration,
            IRepository<RefreshToken> refreshTokenRepository,
            IUserService userService)
        {
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
            _userService = userService;
        }

        public string GenerateAccessToken(AuthUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
            
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "15")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<RefreshToken> GenerateRefreshTokenAsync(string userId)
        {
            var token = GenerateSecureToken();
            var refreshTokenDays = int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7");

            var refreshToken = new RefreshToken
            {
                Token = token,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDays),
                IsRevoked = false
            };

            await _refreshTokenRepository.AddAsync(refreshToken);
            await _refreshTokenRepository.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<Result<(string AccessToken, RefreshToken NewRefreshToken)>> RefreshTokensAsync(string refreshToken)
        {
            var tokens = await _refreshTokenRepository.GetAsync(t => t.Token == refreshToken);
            var existingToken = tokens.FirstOrDefault();

            if (existingToken == null)
                return TokenErrors.InvalidToken;

            if (existingToken.IsRevoked)
                return TokenErrors.RevokedToken;

            if (existingToken.ExpiresAt < DateTime.UtcNow)
                return TokenErrors.ExpiredToken;

            var user = await _userService.GetUserByIdAsync(existingToken.UserId);
            if (user == null)
                return TokenErrors.UserNotFound;

            existingToken.IsRevoked = true;
            existingToken.RevokedReason = "Replaced by new token";

            var newRefreshToken = await GenerateRefreshTokenAsync(existingToken.UserId);
            
            existingToken.ReplacedByToken = newRefreshToken.Token;
            _refreshTokenRepository.Update(existingToken);
            await _refreshTokenRepository.SaveChangesAsync();

            var newAccessToken = GenerateAccessToken(user);

            return (newAccessToken, newRefreshToken);
        }

        public async Task<Result> RevokeRefreshTokenAsync(string refreshToken, string reason = "Logged out")
        {
            var tokens = await _refreshTokenRepository.GetAsync(t => t.Token == refreshToken);
            var existingToken = tokens.FirstOrDefault();

            if (existingToken == null)
                return TokenErrors.InvalidToken;

            if (existingToken.IsRevoked)
                return Result.Success();

            existingToken.IsRevoked = true;
            existingToken.RevokedReason = reason;
            _refreshTokenRepository.Update(existingToken);
            await _refreshTokenRepository.SaveChangesAsync();

            return Result.Success();
        }

        public async Task RevokeAllUserTokensAsync(string userId, string reason = "Security revocation")
        {
            var tokens = await _refreshTokenRepository.GetAsync(t => t.UserId == userId && !t.IsRevoked);

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedReason = reason;
                _refreshTokenRepository.Update(token);
            }

            await _refreshTokenRepository.SaveChangesAsync();
        }

        private static string GenerateSecureToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
