using CleanArc.Application.Commands.Token;
using FluentValidation;

namespace CleanArc.Application.Validations.Token
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.")
                .MaximumLength(200).WithMessage("Refresh token must not exceed 200 characters.");
        }
    }
}
