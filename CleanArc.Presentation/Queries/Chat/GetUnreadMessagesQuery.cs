using CleanArc.Core.Entites;
using MediatR;

namespace CleanArc.Application.Queries.Chat
{
    public class GetUnreadMessagesQuery : IRequest<List<Message>>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
