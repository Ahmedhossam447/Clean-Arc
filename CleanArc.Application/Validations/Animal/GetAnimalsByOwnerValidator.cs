using CleanArc.Application.Queries.Animal;
using FluentValidation;

namespace CleanArc.Application.Validations.Animal
{
     public class GetAnimalsByOwnerValidator :AbstractValidator<GetAnimalsByOwnerQuery>
    {
        public GetAnimalsByOwnerValidator()
        {
            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("Owner ID is required.");
        }
    }
}
