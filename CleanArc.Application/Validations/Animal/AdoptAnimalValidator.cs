using CleanArc.Application.Commands.Animal;
using FluentValidation;

namespace CleanArc.Application.Validations.Animal
{
    public class AdoptAnimalValidator :AbstractValidator<AdoptAnimalCommand>
    {
        public AdoptAnimalValidator()
        {
            RuleFor(x => x.AnimalId)
                .GreaterThan(0).WithMessage("Animal ID must be valid.");
            RuleFor(x => x.AdopterId)
                .NotEmpty().WithMessage("Adopter ID is required.");
        }
    }
}
