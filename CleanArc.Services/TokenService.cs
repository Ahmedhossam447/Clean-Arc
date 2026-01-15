using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CleanArc.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateAccessToken(ApplicationUser user)
        {
          var claims = new[]
          {
              new Claim(JwtRegisteredClaimNames.Sub, user.Id),
              new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? ""),
              new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
          };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                signingCredentials: creds,
                expires: DateTime.UtcNow.AddMinutes(
                double.Parse(_configuration["Jwt:ExpiryMinutes"]!))
                );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
