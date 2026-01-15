using CleanArc.Application.Commands.Request;
using FluentValidation;

namespace CleanArc.Application.Validations.Request
{
    public class CreateRequestCommandValidator : AbstractValidator<CreateRequestCommand>
    {
        public CreateRequestCommandValidator()
        {
            RuleFor(x => x.Userid)
                .NotEmpty().WithMessage("Owner User ID is required.");

            RuleFor(x => x.Useridreq)
                .NotEmpty().WithMessage("Requester User ID is required.");

            RuleFor(x => x.AnimalId)
                .GreaterThan(0).WithMessage("Animal ID must be greater than 0.");

            RuleFor(x => x)
                .Must(x => x.Userid != x.Useridreq)
                .WithMessage("You cannot request to adopt your own animal.");
        }
    }
}
