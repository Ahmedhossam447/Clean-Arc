using CleanArc.Core.Primitives;

namespace CleanArc.Core.Interfaces
{
    public record GoogleUser(string Email, string Name, string Subject);
    public interface IGoogleAuthService
    {
        Task<Result<GoogleUser>> ValidateTokenAsync(string TokenId, CancellationToken cancellationToken = default);
    }
}
