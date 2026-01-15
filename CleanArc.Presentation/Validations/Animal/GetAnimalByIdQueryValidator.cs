using CleanArc.Application.Queries.Animal;
using FluentValidation;

namespace CleanArc.Application.Validations.Animal
{
    public class GetAnimalByIdQueryValidator : AbstractValidator<GetAnimalByIdQuery>
    {
        public GetAnimalByIdQueryValidator()
        {
            RuleFor(x => x.AnimalId)
                .GreaterThan(0).WithMessage("Animal ID must be greater than zero.");
        }
    }
}

