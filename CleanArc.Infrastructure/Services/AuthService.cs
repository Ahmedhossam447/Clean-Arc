using CleanArc.Core.Interfaces;
using CleanArc.Core.Models.Identity;
using CleanArc.Core.Primitives;
using CleanArc.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace CleanArc.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<(bool Succeeded, string? ErrorMessage)> ConfirmEmailAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, "User not found");
            
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, errorMessage);
            }
            
            return (true, null);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return string.Empty;
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return token;
        }

        // Read operations - cancellable
        public async Task<bool> IsEmailConfirmedAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            return await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<AuthUser?> LoginUserAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                return null;
            }
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

        public async Task<(bool Succeeded, string[] Errors)> RegisterUserAsync(
            string username, 
            string password, 
            string email,
            string role = "User",
            string? fullName = null,
            string? photoUrl = null,
            string? location = null,
            string? bio = null,
            string? phoneNumber = null)
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
                Email = email,
                EmailConfirmed = false,
                FullName = fullName,
                PhotoUrl = photoUrl,
                location = location,
                Bio = bio,
                PhoneNumber = phoneNumber
            };

            var result = await _userManager.CreateAsync(newUser, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, role);
                return (true, Array.Empty<string>());
            }

            var errors = result.Errors.Select(e => e.Description).ToArray();
            return (false, errors);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return string.Empty;
            
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, currentPassword);
            if (!isCurrentPasswordValid)
                return false;

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result.Succeeded;
        }
    }
}
