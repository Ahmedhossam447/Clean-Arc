using CleanArc.Application.Contracts.Responses.Animal;
using CleanArc.Core.Primitives;
using MediatR;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Animal;

public class AdoptAnimalCommand : IRequest<Result<AdoptAnimalResponse>>
{
    public int AnimalId { get; set; }

    [JsonIgnore]
    public string AdopterId { get; set; } = string.Empty;
}
