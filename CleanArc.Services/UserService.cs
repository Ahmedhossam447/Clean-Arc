using CleanArc.Core.Interfaces;
using CleanArc.Core.Models.Identity;
using CleanArc.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace CleanArc.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Read operations - cancellable
        public async Task<AuthUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            return await MapToAuthUserAsync(user);
        }

        public async Task<AuthUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            return await MapToAuthUserAsync(user);
        }

        public async Task<AuthUser?> GetProfileAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new AuthUser
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                PhotoUrl = user.PhotoUrl,
                Location = user.location,
                Bio = user.Bio,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.ToList()
            };
        }

        public async Task<bool> UpdateProfileAsync(string userId, string? fullName, string? photoUrl, string? location, string? bio, string? phoneNumber)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.FullName = fullName;
            user.PhotoUrl = photoUrl;
            user.location = location;
            user.Bio = bio;
            user.PhoneNumber = phoneNumber;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        private async Task<AuthUser> MapToAuthUserAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            return new AuthUser
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                PhotoUrl = user.PhotoUrl,
                Location = user.location,
                Bio = user.Bio,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.ToList()
            };
        }
    }
}
