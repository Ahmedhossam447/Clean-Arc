using CleanArc.Application.Commands.Chat;
using CleanArc.Core.Entites;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Handlers.CommandsHandler.Chat
{
    public class MarkMessagesAsReadCommandHandler : IRequestHandler<MarkMessagesAsReadCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public MarkMessagesAsReadCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(MarkMessagesAsReadCommand command, CancellationToken cancellationToken)
        {
            var messageRepo = _unitOfWork.Repository<Message>();
            var messages = await messageRepo.GetAsync(m =>
                m.ReceiverId == command.UserId && 
                m.SenderId == command.SenderId && 
                !m.IsRead, cancellationToken);

            foreach (var message in messages)
            {
                message.IsRead = true;
                messageRepo.Update(message);
            }

            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }
}
