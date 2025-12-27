using CleanArc.Core.Entites;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArc.Core.Entites;

public partial class Request
{
   
    public int Reqid { get; set; }

    public string Userid { get; set; }

    public virtual ApplicationUser? User { get; set; }

    public string Useridreq { get; set; }

    public virtual ApplicationUser? User2 { get; set; }

    public int AnimalId { get; set; }
  
    public virtual Animal? Animal { get; set; }
    public string? Status { get; set; } 

    //public DateTime RequestDate { get; set; } = DateTime.UtcNow;
}