using CleanArc.Application.Commands.Product;
using FluentValidation;

namespace CleanArc.Application.Validations.Product
{
    public class DeleteProductValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Product ID is required and must be greater than 0.");
        }
    }
}
