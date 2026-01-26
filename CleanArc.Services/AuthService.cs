using CleanArc.Core.Interfaces;
using CleanArc.Core.Models.Identity;
using CleanArc.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace CleanArc.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AuthUser?> LoginUserAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                return null;
            }

            // Map ApplicationUser (Infrastructure) to AuthUser (Core)
            var roles = await _userManager.GetRolesAsync(user);
            
            return new AuthUser
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                Roles = roles.ToList()
            };
        }

        public async Task<(bool Succeeded, string[] Errors)> RegisterUserAsync(string username, string password, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, new[] { "Email is required." });
            if (string.IsNullOrWhiteSpace(password))
                return (false, new[] { "Password is required." });
            if (string.IsNullOrWhiteSpace(username))
                return (false, new[] { "Username is required." });

            var existingByEmail = await _userManager.FindByEmailAsync(email);
            if (existingByEmail != null)
            {
                return (false, new[] { "User with this email already exists." });
            }

            var existingByName = await _userManager.FindByNameAsync(username);
            if (existingByName != null)
            {
                return (false, new[] { "Username is already taken." });
            }

            var newUser = new ApplicationUser
            {
                UserName = username,
                Email = email
            };

            var result = await _userManager.CreateAsync(newUser, password);
            if (result.Succeeded)
            {
                return (true, Array.Empty<string>());
            }

            var errors = result.Errors.Select(e => e.Description).ToArray();
            return (false, errors);
        }
    }
}
