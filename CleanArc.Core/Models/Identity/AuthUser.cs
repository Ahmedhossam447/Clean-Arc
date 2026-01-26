namespace CleanArc.Core.Models.Identity
{
    /// <summary>
    /// A pure domain model representing authenticated user data.
    /// This is decoupled from ASP.NET Identity's ApplicationUser.
    /// </summary>
    public class AuthUser
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
