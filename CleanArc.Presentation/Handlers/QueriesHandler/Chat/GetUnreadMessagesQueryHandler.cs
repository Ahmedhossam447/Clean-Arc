using CleanArc.Application.Queries.Chat;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using MediatR;

namespace CleanArc.Application.Handlers.QueriesHandler.Chat
{
    public class GetUnreadMessagesQueryHandler : IRequestHandler<GetUnreadMessagesQuery, List<Message>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUnreadMessagesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Message>> Handle(GetUnreadMessagesQuery query, CancellationToken cancellationToken)
        {
            var messages = await _unitOfWork.Repository<Message>().GetAsync(m =>
                m.ReceiverId == query.UserId && !m.IsRead, cancellationToken);

            return messages
                .OrderBy(m => m.SentAt)
                .ToList();
        }
    }
}
