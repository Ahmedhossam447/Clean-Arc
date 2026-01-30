using CleanArc.Application.Commands.Request;
using FluentValidation;

namespace CleanArc.Application.Validations.Request
{
    public class AcceptRequestCommandValidator : AbstractValidator<AcceptRequestCommand>
    {
        public AcceptRequestCommandValidator()
        {
            RuleFor(x => x.RequestId)
                .GreaterThan(0).WithMessage("Request ID must be greater than zero.");

            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("Owner ID is required.");
        }
    }
}
