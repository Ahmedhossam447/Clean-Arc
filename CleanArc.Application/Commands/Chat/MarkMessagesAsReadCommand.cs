using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Commands.Chat
{
    public class MarkMessagesAsReadCommand : IRequest<Result>
    {
        public string UserId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
    }
}
