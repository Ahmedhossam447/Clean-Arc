using CleanArc.Application.Contracts.Responses.MedicalRecord;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Queries.MedicalRecord
{
    public class GetMedicalRecordByAnimalIdQuery : IRequest<Result<MedicalRecordResponse>>, ICacheableQuery
    {
        [JsonIgnore]
        public int AnimalId { get; set; }

        public string CacheKey => $"medicalrecord:animal:{AnimalId}";
    }
}
