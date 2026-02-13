using CleanArc.Core.Entities;
using MediatR;

namespace CleanArc.Application.Queries.Chat
{
    public class GetChatHistoryQuery : IRequest<List<Message>>
    {
        public string UserId { get; set; } = string.Empty;
        public string OtherUserId { get; set; } = string.Empty;
        public DateTime? BeforeDate { get; set; }
        public int Limit { get; set; } = 50;
    }
}
