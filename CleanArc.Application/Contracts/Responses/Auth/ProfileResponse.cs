namespace CleanArc.Application.Contracts.Responses.Auth
{
    public class ProfileResponse
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Location { get; set; }
        public string? Bio { get; set; }
        public string? PhoneNumber { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
