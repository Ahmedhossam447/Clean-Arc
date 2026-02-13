namespace CleanArc.Application.Contracts.Responses.Animal
{
    public class UpdateAnimalResponse
    {
        public int AnimalId { get; set; }

        public string? Name { get; set; }

        public byte? Age { get; set; }

        public string? Type { get; set; }

        public string? Breed { get; set; }

        public string? Gender { get; set; }

        public string? Photo { get; set; }
        public bool IsAdopted { get; set; } = false;
        public string? About { get; set; }

        public string? OwnerId { get; set; }
    }
}
