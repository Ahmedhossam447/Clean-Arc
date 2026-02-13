using CleanArc.Application.Queries.Animal;
using FluentValidation;

namespace CleanArc.Application.Validations.Animal
{
    public class SearchAnimalsQueryValidator : AbstractValidator<SearchAnimalsQuery>
    {
        public SearchAnimalsQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than zero.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than zero.")
                .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100.");
        }
    }
}
