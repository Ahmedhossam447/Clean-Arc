using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Interfaces;
using CleanArc.Core.Primitives;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Queries.Animal
{
    public class GetAnimalByIdQuery : IRequest<Result<ReadAnimalResponse>>, ICacheableQuery
    {
        [JsonIgnore]
        public int AnimalId { get; set; }

        public string CacheKey => $"animal:{AnimalId}";
    }
}

