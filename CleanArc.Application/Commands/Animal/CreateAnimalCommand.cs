using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Primitives;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Animal
{
    public class CreateAnimalCommand : IRequest<Result<CreateAnimalResponse>>
    {
        public string? Name { get; set; }

        public byte? Age { get; set; }

        public string? Type { get; set; }

        public string? Breed { get; set; }

        public string? Gender { get; set; }
        [JsonIgnore]
        public Stream? Photo { get; set; }
        [JsonIgnore]
        public string fileName { get; set; } = string.Empty;
        public string? About { get; set; }

        public string? OwnerId { get; set; }

        // MedicalRecord properties
        public double Weight { get; set; }
        public double Height { get; set; }
        public string? BloodType { get; set; }
        public string? MedicalHistoryNotes { get; set; }
        public string? Injuries { get; set; }
        public string? Status { get; set; }
    }
}
