using CleanArc.Application.Commands.Animal;
using FluentValidation;

namespace CleanArc.Application.Validations.Animal
{
    public class CreateAnimalValidator : AbstractValidator<CreateAnimalCommand>
    {
        public CreateAnimalValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            RuleFor(x => x.Age)
                .InclusiveBetween((byte)1, (byte)50).WithMessage("Age must be between 1 and 50.");
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required.")
                .MaximumLength(50).WithMessage("Type cannot exceed 50 characters.");
            RuleFor(x => x.Breed)
                .NotEmpty().WithMessage("Breed is required.")
                .MaximumLength(50).WithMessage("Breed cannot exceed 50 characters.");
            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required.");
            RuleFor(x => x.About).MaximumLength(500).WithMessage("About cannot exceed 500 characters.");
            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("User ID is required.");
            RuleFor(x => x.Photo).Cascade(CascadeMode.Stop).NotNull().WithMessage("Photo is required.").Must(p=>p.Length <= 5 * 1024 * 1024)
                .WithMessage("Photo size must be less than or equal to 5 MB.");

            RuleFor(p => p.fileName)
                    .Must(fileName => fileName != null && (fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                                           fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                                           fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)))
                    .WithMessage("Photo must be a .jpg, .jpeg, or .png file.");
            
            // MedicalRecord validations
            RuleFor(x => x.Weight)
                .GreaterThanOrEqualTo(0).WithMessage("Weight must be greater than or equal to 0.");
            RuleFor(x => x.Height)
                .GreaterThanOrEqualTo(0).WithMessage("Height must be greater than or equal to 0.");
            RuleFor(x => x.BloodType)
                .MaximumLength(50).WithMessage("Blood type cannot exceed 50 characters.");
            RuleFor(x => x.MedicalHistoryNotes)
                .MaximumLength(2000).WithMessage("Medical history notes cannot exceed 2000 characters.");
            RuleFor(x => x.Injuries)
                .MaximumLength(500).WithMessage("Injuries cannot exceed 500 characters.");
            RuleFor(x => x.Status)
                .MaximumLength(100).WithMessage("Status cannot exceed 100 characters.");
        }
    }
}
