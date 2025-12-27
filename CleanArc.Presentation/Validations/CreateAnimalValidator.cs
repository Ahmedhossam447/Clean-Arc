using CleanArc.Application.Commands;
using FluentValidation;

namespace CleanArc.Application.Validations
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
            //RuleFor(x => x.Userid)
              //  .NotEmpty().WithMessage("User ID is required.");
              RuleFor(x => x.Photo)
                .MaximumLength(2000).WithMessage("Photo URL cannot exceed 2000 characters.")
                .NotEmpty().WithMessage("photo is required");
        }
    }
}
