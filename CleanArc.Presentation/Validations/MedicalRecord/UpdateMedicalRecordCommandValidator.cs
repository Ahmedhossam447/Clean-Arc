using CleanArc.Application.Commands.MedicalRecord;
using FluentValidation;

namespace CleanArc.Application.Validations.MedicalRecord
{
    public class UpdateMedicalRecordCommandValidator : AbstractValidator<UpdateMedicalRecordCommand>
    {
        public UpdateMedicalRecordCommandValidator()
        {
            RuleFor(x => x.AnimalId)
                .GreaterThan(0).WithMessage("Animal ID must be greater than 0.");

            RuleFor(x => x.Weight)
                .GreaterThanOrEqualTo(0).WithMessage("Weight must be greater than or equal to 0.")
                .When(x => x.Weight.HasValue);

            RuleFor(x => x.Height)
                .GreaterThanOrEqualTo(0).WithMessage("Height must be greater than or equal to 0.")
                .When(x => x.Height.HasValue);

            RuleFor(x => x.BloodType)
                .MaximumLength(50).WithMessage("Blood type cannot exceed 50 characters.")
                .When(x => !string.IsNullOrEmpty(x.BloodType));

            RuleFor(x => x.MedicalHistoryNotes)
                .MaximumLength(2000).WithMessage("Medical history notes cannot exceed 2000 characters.")
                .When(x => !string.IsNullOrEmpty(x.MedicalHistoryNotes));

            RuleFor(x => x.Injuries)
                .MaximumLength(500).WithMessage("Injuries cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.Injuries));

            RuleFor(x => x.Status)
                .MaximumLength(100).WithMessage("Status cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Status));
        }
    }
}
