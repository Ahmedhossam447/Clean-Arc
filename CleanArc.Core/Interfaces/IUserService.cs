using CleanArc.Core.Models.Identity;

namespace CleanArc.Core.Interfaces
{
    public interface IUserService
    {
        Task<AuthUser?> GetUserByIdAsync(string userId);
        Task<AuthUser?> GetUserByEmailAsync(string email);
        Task<AuthUser?> GetProfileAsync(string userId);
        Task<bool> UpdateProfileAsync(string userId, string? fullName, string? photoUrl, string? location, string? bio, string? phoneNumber);
    }
}
