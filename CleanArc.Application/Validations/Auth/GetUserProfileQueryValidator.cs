using CleanArc.Application.Queries.Auth;
using FluentValidation;

namespace CleanArc.Application.Validations.Auth
{
    public class GetUserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
    {
        public GetUserProfileQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");
        }
    }
}
