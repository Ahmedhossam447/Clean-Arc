using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArc.Application.Contracts.Responses.Animal
{
    public class ReadAnimalResponse
    {
        public int AnimalId { get; set; }
        public string? Name { get; set; }

        public byte? Age { get; set; }

        public string? Type { get; set; }

        public string? Breed { get; set; }

        public string? Gender { get; set; }

        public string? Photo { get; set; }
        public string? About { get; set; }
        public bool IsAdopted { get; set; }

        public string? Userid { get; set; }
    }
}
