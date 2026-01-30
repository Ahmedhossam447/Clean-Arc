using Clean_Arc.Hubs;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace CleanArc
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IRepository<UserConnection> _userConnectionRepository;

        public NotificationService(
            IHubContext<ChatHub> hubContext,
            IRepository<UserConnection> userConnectionRepository)
        {
            _hubContext = hubContext;
            _userConnectionRepository = userConnectionRepository;
        }

        public async Task SendNotificationAsync(string userId, string type, object data)
        {
            var connections = await _userConnectionRepository.GetAsync(c => c.UserId == userId);
            foreach (var conn in connections)
            {
                await _hubContext.Clients.Client(conn.ConnectionId).SendAsync("ReceiveNotification", type, data);
            }
        }
    }
}
