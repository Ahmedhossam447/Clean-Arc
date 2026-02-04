namespace Clean_Arc.Contracts.Request
{
    using Microsoft.AspNetCore.Http;
    public class UpdateAnimalRequest
    {
        public string? Name { get; set; }
        public byte? Age { get; set; }
        public string? Type { get; set; }
        public string? Breed { get; set; }
        public string? Gender { get; set; }
        public string? About { get; set; }

        // The File Input
        public IFormFile? Photo { get; set; }
    }
}
