using CleanArc.Application.Commands.Animal;
using FluentValidation;

namespace CleanArc.Application.Validations.Animal
{
    public class DeleteAnimalValidator : AbstractValidator<DeleteAnimalCommand>
    {
        public DeleteAnimalValidator()
        {
            RuleFor(x => x.AnimalId)
                .GreaterThan(0).WithMessage("Animal ID is required and must be greater than 0.");
        }
    }
}
