namespace CleanArc.Application.Contracts.Responses.Request
{
    public class CreateRequestResponse
    {
        public int Id { get; set; }
        public string? OwnerId { get; set; }
        public string? RequesterId { get; set; }
        public int AnimalId { get; set; }
        public string? Status { get; set; }
    }
}
