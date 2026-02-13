using CleanArc.Application.Queries.MedicalRecord;
using FluentValidation;

namespace CleanArc.Application.Validations.MedicalRecord
{
    public class GetMedicalRecordByAnimalIdQueryValidator : AbstractValidator<GetMedicalRecordByAnimalIdQuery>
    {
        public GetMedicalRecordByAnimalIdQueryValidator()
        {
            RuleFor(x => x.AnimalId)
                .GreaterThan(0).WithMessage("Animal ID must be greater than 0.");
        }
    }
}
