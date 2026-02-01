using CleanArc.Application.Contracts.Responses.MedicalRecord;
using CleanArc.Core.Primitives;
using MediatR;

namespace CleanArc.Application.Commands.Vaccination
{
    public class AddVaccinationCommand : IRequest<Result<VaccinationResponse>>
    {
        public int AnimalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime DateGiven { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
