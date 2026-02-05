using CleanArc.Core.Interfaces;
using CleanArc.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CleanArc.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public NotificationService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationAsync(List<string> userId, string type, object data)
        {
            await _hubContext.Clients.Users(userId).SendAsync("ReceiveNotification", type, data);
        }
        public async Task SendNotificationToUserAsync(string userId, string type, object data)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", type, data);
        }
    }
}
