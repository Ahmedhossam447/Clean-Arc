using CleanArc.Application.Commands.Vaccination;
using FluentValidation;

namespace CleanArc.Application.Validations.Vaccination
{
    public class DeleteVaccinationCommandValidator : AbstractValidator<DeleteVaccinationCommand>
    {
        public DeleteVaccinationCommandValidator()
        {
            RuleFor(x => x.VaccinationId)
                .GreaterThan(0).WithMessage("Vaccination ID must be greater than 0.");
        }
    }
}
