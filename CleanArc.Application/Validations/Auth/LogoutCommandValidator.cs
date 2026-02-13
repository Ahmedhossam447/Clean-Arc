using CleanArc.Application.Commands.Auth;
using FluentValidation;

namespace CleanArc.Application.Validations.Auth
{
    public class LogoutCommandValidator :AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.")
                .MaximumLength(200).WithMessage("Refresh token must not exceed 200 characters.");
        }

    }
}
