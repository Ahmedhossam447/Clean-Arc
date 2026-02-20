using CleanArc.Application.Commands.Auth;
using FluentValidation;

namespace CleanArc.Application.Validations.Auth
{
    public class GoogleLoginCommandValidator : AbstractValidator<GoogleLoginCommand>
    {
        public GoogleLoginCommandValidator()
        {
            RuleFor(x => x.TokenId)
                .NotEmpty().WithMessage("TokenId is required.")
                .NotNull().WithMessage("TokenId cannot be null.");
        }
    }
}
