using CleanArc.Core.Models.Identity;

namespace CleanArc.Core.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Succeeded, string[] Errors)> RegisterUserAsync(string username, string password, string email);
        
        /// <summary>
        /// Validates credentials and returns the authenticated user data.
        /// Returns null if login fails.
        /// </summary>
        Task<AuthUser?> LoginUserAsync(string email, string password);
    }
}
