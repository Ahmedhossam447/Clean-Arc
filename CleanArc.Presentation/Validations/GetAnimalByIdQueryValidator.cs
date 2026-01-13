using CleanArc.Application.Queries;
using FluentValidation;

namespace CleanArc.Application.Validations
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

