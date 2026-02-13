using CleanArc.Application.Queries.Chat;
using FluentValidation;

namespace CleanArc.Application.Validations.Chat
{
    public class GetUnreadMessagesQueryValidator : AbstractValidator<GetUnreadMessagesQuery>
    {
        public GetUnreadMessagesQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");
        }
    }
}
