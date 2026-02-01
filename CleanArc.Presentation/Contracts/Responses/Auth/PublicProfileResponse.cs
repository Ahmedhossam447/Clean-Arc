namespace CleanArc.Application.Contracts.Responses.Auth
{
    public class PublicProfileResponse
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Location { get; set; }
        public string? Bio { get; set; }
        // Note: Email and PhoneNumber are excluded for privacy
        // Roles might be included if needed for public display
    }
}
