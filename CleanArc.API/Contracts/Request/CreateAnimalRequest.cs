namespace CleanArc.API.Contracts.Request
{
    using Microsoft.AspNetCore.Http;
        public class CreateAnimalRequest
        {
            public string? Name { get; set; }
            public byte? Age { get; set; }
            public string? Type { get; set; }
            public string? Breed { get; set; }
            public string? Gender { get; set; }
            public string? About { get; set; }
            public string? OwnerId { get; set; }

            // Medical info
            public double Weight { get; set; }
            public double Height { get; set; }
            public string? BloodType { get; set; }
            public string? MedicalHistoryNotes { get; set; }
            public string? Injuries { get; set; }
            public string? Status { get; set; }

            // The File Input
            public IFormFile? Photo { get; set; }
        }
    }
