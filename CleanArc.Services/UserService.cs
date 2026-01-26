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

        public async Task<AuthUser?> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            return await MapToAuthUserAsync(user);
        }

        public async Task<AuthUser?> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            return await MapToAuthUserAsync(user);
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
                Roles = roles.ToList()
            };
        }
    }
}
