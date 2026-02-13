namespace CleanArc.Application.Contracts.Responses.Auth
{
    public class LoginResponse
    {
        public bool Succeeded { get; set; }
        public List<string> Errors { get; set; } = new();
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
