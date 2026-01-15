using CleanArc.Application.Contracts.Responses.Animal;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CleanArc.Application.Commands.Animal
{
    public class AdoptAnimalCommand :IRequest<AdoptAnimalResponse>
    {
        public int AnimalId { get; set; }
        [JsonIgnore]
        public string AdopterId { get; set; }
    }
}
