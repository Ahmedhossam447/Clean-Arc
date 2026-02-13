using CleanArc.Application.Commands.Vaccination;
using FluentValidation;

namespace CleanArc.Application.Validations.Vaccination
{
    public class AddVaccinationCommandValidator : AbstractValidator<AddVaccinationCommand>
    {
        public AddVaccinationCommandValidator()
        {
            RuleFor(x => x.AnimalId)
                .GreaterThan(0).WithMessage("Animal ID must be greater than 0.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Vaccination name is required.")
                .MaximumLength(100).WithMessage("Vaccination name cannot exceed 100 characters.");

            RuleFor(x => x.DateGiven)
                .NotEmpty().WithMessage("Date given is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Date given cannot be in the future.");

            RuleFor(x => x.ExpiryDate)
                .NotEmpty().WithMessage("Expiry date is required.")
                .GreaterThan(x => x.DateGiven).WithMessage("Expiry date must be after the date given.");
        }
    }
}
