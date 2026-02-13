using CleanArc.Application.Contracts.Responses.MedicalRecord;
using CleanArc.Core.Primitives;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Vaccination
{
    public class UpdateVaccinationCommand : IRequest<Result<VaccinationResponse>>
    {
        [JsonIgnore]
        public int VaccinationId { get; set; }

        public string? Name { get; set; }
        public DateTime? DateGiven { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
