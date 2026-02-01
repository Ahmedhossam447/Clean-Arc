using CleanArc.Core.Models.Identity;

namespace CleanArc.Core.Interfaces
{
    public interface IUserService
    {
        // Read operations - cancellable
        Task<AuthUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<AuthUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<AuthUser?> GetProfileAsync(string userId, CancellationToken cancellationToken = default);
        
        // Write operations - not cancellable
        Task<bool> UpdateProfileAsync(string userId, string? fullName, string? photoUrl, string? location, string? bio, string? phoneNumber);
    }
}
