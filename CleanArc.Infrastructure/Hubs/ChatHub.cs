using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace CleanArc.Infrastructure.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChatHub(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task OnConnectedAsync()
        {
            // Verify user is authenticated
            var userId = Context.User?.FindFirst("sub")?.Value 
                ?? Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                Context.Abort();
                return;
            }

            // SignalR automatically maps this connection to the user via IUserIdProvider
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string receiverId, string message)
        {
            var senderId = Context.User?.FindFirst("sub")?.Value 
                ?? Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(senderId))
            {
                Context.Abort();
                return;
            }

            var messageEntity = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = message,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            await _unitOfWork.Repository<Message>().AddAsync(messageEntity);
            await _unitOfWork.SaveChangesAsync();

            // SignalR automatically finds all active connections for these users!
            // No database query needed - it uses the in-memory mapping
            await Clients.User(receiverId).SendAsync("ReceiveMessage", messageEntity);
            await Clients.User(senderId).SendAsync("SenderMessage", messageEntity);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // SignalR automatically removes the connection from its in-memory mapping
            // No manual cleanup needed!
            await base.OnDisconnectedAsync(exception);
        }
    }
}
