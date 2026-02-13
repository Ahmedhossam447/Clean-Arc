namespace CleanArc.Application.Contracts.Responses.Request
{
    public class RequestResponse
    {
        public int Id { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public string RequesterId { get; set; } = string.Empty;
        public int AnimalId { get; set; }
        public string? AnimalName { get; set; }
        public string? Status { get; set; }
    }
}
