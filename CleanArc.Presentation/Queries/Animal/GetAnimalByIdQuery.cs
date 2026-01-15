using CleanArc.Application.Contracts.Responses.Animal;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Queries.Animal
{
    public class GetAnimalByIdQuery : IRequest<ReadAnimalResponse>
    {
        [JsonIgnore]
        public int AnimalId { get; set; }
    }
}

