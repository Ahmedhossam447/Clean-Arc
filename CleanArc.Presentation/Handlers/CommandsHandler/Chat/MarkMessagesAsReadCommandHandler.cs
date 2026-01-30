using CleanArc.Application.Commands.Chat;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.CommandsHandler.Chat
{
    public class MarkMessagesAsReadCommandHandler : IRequestHandler<MarkMessagesAsReadCommand, Result>
    {
        private readonly IRepository<Message> _messageRepository;

        public MarkMessagesAsReadCommandHandler(IRepository<Message> messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<Result> Handle(MarkMessagesAsReadCommand command, CancellationToken cancellationToken)
        {
            var messages = await _messageRepository.GetAsync(m =>
                m.ReceiverId == command.UserId && 
                m.SenderId == command.SenderId && 
                !m.IsRead);

            foreach (var message in messages)
            {
                message.IsRead = true;
                _messageRepository.Update(message);
            }

            await _messageRepository.SaveChangesAsync();

            return Result.Success();
        }
    }
}
