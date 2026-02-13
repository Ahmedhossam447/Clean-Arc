using CleanArc.Application.Contracts.Responses.Animal;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Animal
{
    public class UpdateAnimalCommand : IRequest<UpdateAnimalResponse>
    {
        [JsonIgnore]
        public int AnimalId { get; set; }

        [JsonIgnore]
        public string? UserId { get; set; }

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
    }
}
