using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Primitives;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Animal
{
    public class DeleteAnimalCommand : IRequest<Result<DeleteAnimalResponse>>
    {
        [JsonIgnore]
        public int AnimalId { get; set; }

        [JsonIgnore]
        public string? UserId { get; set; }
    }
}
