using CleanArc.Core.Models.Identity;

namespace CleanArc.Core.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Succeeded, string[] Errors)> RegisterUserAsync(string username, string password, string email);
        Task<AuthUser?> LoginUserAsync(string email, string password);
    }
}
