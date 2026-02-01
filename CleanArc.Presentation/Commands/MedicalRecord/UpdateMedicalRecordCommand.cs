using CleanArc.Application.Contracts.Responses.MedicalRecord;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.MedicalRecord
{
    public class UpdateMedicalRecordCommand : IRequest<MedicalRecordResponse>
    {
        [JsonIgnore]
        public int AnimalId { get; set; }

        public double? Weight { get; set; }
        public double? Height { get; set; }
        public string? BloodType { get; set; }
        public string? MedicalHistoryNotes { get; set; }
        public string? Injuries { get; set; }
        public string? Status { get; set; }
    }
}
