using CleanArc.Core.Entities;
using MediatR;

namespace CleanArc.Application.Queries.Chat
{
    public class GetUnreadMessagesQuery : IRequest<List<Message>>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
