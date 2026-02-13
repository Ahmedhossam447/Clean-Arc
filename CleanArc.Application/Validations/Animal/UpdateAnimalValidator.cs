using CleanArc.Application.Commands.Animal;
using FluentValidation;

namespace CleanArc.Application.Validations.Animal
{
    public class UpdateAnimalValidator : AbstractValidator<UpdateAnimalCommand>
    {
        public UpdateAnimalValidator()
        {
            RuleFor(x => x.AnimalId)
                .GreaterThan(0).WithMessage("Animal ID is required and must be greater than 0.");
            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            RuleFor(x => x.Age)
                .InclusiveBetween((byte)1, (byte)50).WithMessage("Age must be between 1 and 50.");
            RuleFor(x => x.Type)
                .MaximumLength(50).WithMessage("Type cannot exceed 50 characters.");
            RuleFor(x => x.Breed)
                .MaximumLength(50).WithMessage("Breed cannot exceed 50 characters.");
            RuleFor(x => x.Gender);
            RuleFor(x => x.About).MaximumLength(500).WithMessage("About cannot exceed 500 characters.");
            RuleFor(x => x.Photo).Cascade(CascadeMode.Stop).NotNull().WithMessage("Photo is required.").Must(p => p.Length <= 5 * 1024 * 1024)
    .WithMessage("Photo size must be less than or equal to 5 MB.");

            RuleFor(p => p.fileName)
                    .Must(fileName => fileName != null && (fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                                           fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                                           fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)))
                    .WithMessage("Photo must be a .jpg, .jpeg, or .png file.");


        }
    }
}
