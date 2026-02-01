using CleanArc.Application.Queries.Chat;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.Chat
{
    public class GetChatHistoryQueryHandler : IRequestHandler<GetChatHistoryQuery, List<Message>>
    {
        private readonly IRepository<Message> _messageRepository;

        public GetChatHistoryQueryHandler(IRepository<Message> messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<List<Message>> Handle(GetChatHistoryQuery query, CancellationToken cancellationToken)
        {
            var beforeDate = query.BeforeDate ?? DateTime.UtcNow;

            var messages = await _messageRepository.GetAsync(m =>
                ((m.SenderId == query.UserId && m.ReceiverId == query.OtherUserId) ||
                (m.SenderId == query.OtherUserId && m.ReceiverId == query.UserId)) &&
                m.SentAt < beforeDate, cancellationToken);

            return messages
                .OrderByDescending(m => m.SentAt)
                .Take(query.Limit)
                .OrderBy(m => m.SentAt)
                .ToList();
        }
    }
}
