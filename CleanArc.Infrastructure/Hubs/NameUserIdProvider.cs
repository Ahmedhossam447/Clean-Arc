using Microsoft.AspNetCore.SignalR;

namespace CleanArc.Infrastructure.Hubs
{
    public class NameUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            // SignalR will use this to extract user ID from JWT claims
            // First try "sub" claim (JWT standard), then fall back to NameIdentifier
            return connection.User?.FindFirst("sub")?.Value 
                ?? connection.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
