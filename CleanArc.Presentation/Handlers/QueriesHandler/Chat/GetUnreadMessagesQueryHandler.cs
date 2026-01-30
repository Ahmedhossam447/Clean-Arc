using CleanArc.Application.Queries.Chat;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.Chat
{
    public class GetUnreadMessagesQueryHandler : IRequestHandler<GetUnreadMessagesQuery, List<Message>>
    {
        private readonly IRepository<Message> _messageRepository;

        public GetUnreadMessagesQueryHandler(IRepository<Message> messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<List<Message>> Handle(GetUnreadMessagesQuery query, CancellationToken cancellationToken)
        {
            var messages = await _messageRepository.GetAsync(m =>
                m.ReceiverId == query.UserId && !m.IsRead);

            return messages
                .OrderBy(m => m.SentAt)
                .ToList();
        }
    }
}
