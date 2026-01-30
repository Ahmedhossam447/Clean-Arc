using CleanArc.Application.Commands.Request;
using FluentValidation;

namespace CleanArc.Application.Validations.Request
{
    public class CreateRequestCommandValidator : AbstractValidator<CreateRequestCommand>
    {
        public CreateRequestCommandValidator()
        {
            RuleFor(x => x.AnimalId)
                .GreaterThan(0).WithMessage("Animal ID must be greater than 0.");

            RuleFor(x => x.RequesterId)
                .NotEmpty().WithMessage("Requester ID is required.");
        }
    }
}
