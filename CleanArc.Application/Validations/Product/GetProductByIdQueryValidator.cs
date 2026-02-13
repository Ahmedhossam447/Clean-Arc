using CleanArc.Application.Queries.Product;
using FluentValidation;

namespace CleanArc.Application.Validations.Product
{
    public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
    {
        public GetProductByIdQueryValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Product ID must be greater than zero.");
        }
    }
}
