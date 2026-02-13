using CleanArc.Application.Commands.Product;
using FluentValidation;

namespace CleanArc.Application.Validations.Product
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Product ID is required and must be greater than 0.");

            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).When(x => x.Price.HasValue)
                .WithMessage("Price must be greater than 0.");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).When(x => x.StockQuantity.HasValue)
                .WithMessage("Stock quantity cannot be negative.");

            When(x => x.Image != null, () =>
            {
                RuleFor(x => x.Image)
                    .Must(p => p!.Length <= 5 * 1024 * 1024)
                    .WithMessage("Image size must be less than or equal to 5 MB.");

                RuleFor(x => x.FileName)
                    .Must(fileName => fileName != null &&
                        (fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                         fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                         fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)))
                    .WithMessage("Image must be a .jpg, .jpeg, or .png file.");
            });
        }
    }
}
