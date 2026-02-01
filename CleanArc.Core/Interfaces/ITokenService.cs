using CleanArc.Core.Entities;
using CleanArc.Core.Models.Identity;
using CleanArc.Core.Primitives;

namespace CleanArc.Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(AuthUser user);
        
        Task<RefreshToken> GenerateRefreshTokenAsync(string userId);
        
        // Read operation - cancellable
        Task<Result<(string AccessToken, RefreshToken NewRefreshToken)>> RefreshTokensAsync(string refreshToken, CancellationToken cancellationToken = default);
        
        // Write operations - not cancellable
        Task<Result> RevokeRefreshTokenAsync(string refreshToken, string reason = "Logged out");
        
        Task RevokeAllUserTokensAsync(string userId, string reason = "Security revocation");
    }
}
