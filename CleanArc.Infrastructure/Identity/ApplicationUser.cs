using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CleanArc.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
  
        public string? PhotoUrl { get; set; }

        public string? FullName { get; set; }

        public string? location { get; set; }
        public string? Bio { get; set; }
    }
}

