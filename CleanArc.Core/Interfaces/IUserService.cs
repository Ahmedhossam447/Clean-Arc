using CleanArc.Core.Models.Identity;

namespace CleanArc.Core.Interfaces
{
    public interface IUserService
    {
        Task<AuthUser?> GetUserByIdAsync(string userId);
        Task<AuthUser?> GetUserByEmailAsync(string email);
    }
}
