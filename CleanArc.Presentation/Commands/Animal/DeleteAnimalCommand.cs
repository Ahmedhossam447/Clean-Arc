using CleanArc.Application.Contracts.Responses.Animal;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Animal
{
    public class DeleteAnimalCommand: IRequest<DeleteAnimalResponse>
    {
        [JsonIgnore]
        public int AnimalId { get; set; }
    }
}
