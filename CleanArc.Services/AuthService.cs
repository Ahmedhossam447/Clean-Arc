using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
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
        public async Task<ApplicationUser> LoginUserAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                return user;
            }
            return null;
        }

        public async Task<(bool Succeeded, string[] Errors)> RegisterUserAsync(string username, string password, string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, new string[] { "Email is required." });
            if (string.IsNullOrWhiteSpace(password))
                return (false, new string[] { "Password is required." });
            if (string.IsNullOrWhiteSpace(username))
                return (false, new string[] { "Username is required." });
            var user =await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                return (false, new string[] { "User with this email already exists." });
            }
            var existingUser = await _userManager.FindByNameAsync(username);
            if (existingUser != null)
            {
                return (false, new string[] { "Username is already taken." });
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
            else
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return (false, errors);
            }
        }
    }
}
