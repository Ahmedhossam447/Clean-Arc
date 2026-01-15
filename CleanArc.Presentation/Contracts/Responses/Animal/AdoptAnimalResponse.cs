using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Contracts.Responses.Animal
{
    public class AdoptAnimalResponse
    {
        public bool Succeeded { get; set; }
        public int animalId { get; set; }
    }
}
