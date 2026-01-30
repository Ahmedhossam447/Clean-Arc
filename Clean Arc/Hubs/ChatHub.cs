using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Clean_Arc.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IRepository<UserConnection> _userConnectionRepository;
        private readonly IRepository<Message> _messageRepository;

        public ChatHub(
            IRepository<UserConnection> userConnectionRepository,
            IRepository<Message> messageRepository)
        {
            _userConnectionRepository = userConnectionRepository;
            _messageRepository = messageRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var userid = Context.User?.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userid))
            {
                Context.Abort();
                return;
            }

            var connection = new UserConnection
            {
                UserId = userid,
                ConnectionId = Context.ConnectionId
            };

            await _userConnectionRepository.AddAsync(connection);
            await _userConnectionRepository.SaveChangesAsync();
            await base.OnConnectedAsync();
        }

        public async Task SendMessage(string receiverId, string message)
        {
            var senderId = Context.User?.FindFirst("sub")?.Value;
            
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

            await _messageRepository.AddAsync(messageEntity);
            await _messageRepository.SaveChangesAsync();

            var receiverConnections = await _userConnectionRepository.GetAsync(c => c.UserId == receiverId);
            var senderConnections = await _userConnectionRepository.GetAsync(c => c.UserId == senderId);

            foreach (var conn in receiverConnections)
            {
                await Clients.Client(conn.ConnectionId).SendAsync("ReceiveMessage", messageEntity);
            }
            foreach (var conn in senderConnections)
            {
                await Clients.Client(conn.ConnectionId).SendAsync("SenderMessage", messageEntity);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            var connections = await _userConnectionRepository.GetAsync(c => c.ConnectionId == connectionId);
            var connection = connections.FirstOrDefault();

            if (connection != null)
            {
                await _userConnectionRepository.Delete(connection.Id);
                await _userConnectionRepository.SaveChangesAsync();
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
