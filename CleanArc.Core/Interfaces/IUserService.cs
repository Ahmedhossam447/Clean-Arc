using CleanArc.Core.Models.Identity;

namespace CleanArc.Core.Interfaces
{
    /// <summary>
    /// Abstracts user lookup operations for the Application layer.
    /// Implementation lives in Services/Infrastructure.
    /// </summary>
    public interface IUserService
    {
        Task<AuthUser?> GetUserByIdAsync(string userId);
        Task<AuthUser?> GetUserByEmailAsync(string email);
    }
}
