using CleanArc.Application.Commands.Vaccination;
using FluentValidation;

namespace CleanArc.Application.Validations.Vaccination
{
    public class UpdateVaccinationCommandValidator : AbstractValidator<UpdateVaccinationCommand>
    {
        public UpdateVaccinationCommandValidator()
        {
            RuleFor(x => x.VaccinationId)
                .GreaterThan(0).WithMessage("Vaccination ID must be greater than 0.");

            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Vaccination name cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.DateGiven)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Date given cannot be in the future.")
                .When(x => x.DateGiven.HasValue);

            RuleFor(x => x.ExpiryDate)
                .GreaterThan(x => x.DateGiven ?? DateTime.MinValue).WithMessage("Expiry date must be after the date given.")
                .When(x => x.ExpiryDate.HasValue && x.DateGiven.HasValue);
        }
    }
}
