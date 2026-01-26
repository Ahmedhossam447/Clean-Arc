using CleanArc.Core.Models.Identity;

namespace CleanArc.Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(AuthUser user);
    }
}
