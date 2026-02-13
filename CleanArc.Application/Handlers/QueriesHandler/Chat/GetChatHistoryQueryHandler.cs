using CleanArc.Application.Queries.Chat;
using CleanArc.Core.Entities;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.Chat
{
    public class GetChatHistoryQueryHandler : IRequestHandler<GetChatHistoryQuery, List<Message>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetChatHistoryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Message>> Handle(GetChatHistoryQuery query, CancellationToken cancellationToken)
        {
            var beforeDate = query.BeforeDate ?? DateTime.UtcNow;

            var messages = await _unitOfWork.Repository<Message>().GetAsync(m =>
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
