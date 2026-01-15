using CleanArc.Application.Contracts.Responses.Animal;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Animal
{
    public class UpdateAnimalCommand : IRequest<UpdateAnimalResponse>
    {
        [JsonIgnore]
        public int AnimalId { get; set; }

        public string? Name { get; set; }
        public byte? Age { get; set; }
        public string? Type { get; set; }
        public string? Breed { get; set; }
        public string? Gender { get; set; }
        public string? Photo { get; set; }
        public string? About { get; set; }
    }
}
