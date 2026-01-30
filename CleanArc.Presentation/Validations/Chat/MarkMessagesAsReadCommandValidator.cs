using CleanArc.Application.Commands.Chat;
using FluentValidation;

namespace CleanArc.Application.Validations.Chat
{
    public class MarkMessagesAsReadCommandValidator : AbstractValidator<MarkMessagesAsReadCommand>
    {
        public MarkMessagesAsReadCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.SenderId)
                .NotEmpty().WithMessage("Sender ID is required.");
        }
    }
}
