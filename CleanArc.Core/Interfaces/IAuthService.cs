using CleanArc.Core.Models.Identity;

namespace CleanArc.Core.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Succeeded, string[] Errors)> RegisterUserAsync(
            string username, 
            string password, 
            string email,
            string? fullName = null,
            string? photoUrl = null,
            string? location = null,
            string? bio = null,
            string? phoneNumber = null);
        
        // Read operations - cancellable
        Task<AuthUser?> LoginUserAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<bool> IsEmailConfirmedAsync(string email, CancellationToken cancellationToken = default);
        
        // Write operations - not cancellable
        Task<string> GenerateEmailConfirmationTokenAsync(string email);
        Task<(bool Succeeded, string? ErrorMessage)> ConfirmEmailAsync(string email, string token);
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    }
}
