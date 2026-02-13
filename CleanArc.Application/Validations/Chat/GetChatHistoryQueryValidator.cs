using CleanArc.Application.Queries.Chat;
using FluentValidation;

namespace CleanArc.Application.Validations.Chat
{
    public class GetChatHistoryQueryValidator : AbstractValidator<GetChatHistoryQuery>
    {
        public GetChatHistoryQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.OtherUserId)
                .NotEmpty().WithMessage("Other User ID is required.");

            RuleFor(x => x.Limit)
                .GreaterThan(0).WithMessage("Limit must be greater than zero.")
                .LessThanOrEqualTo(100).WithMessage("Limit cannot exceed 100.");
        }
    }
}
