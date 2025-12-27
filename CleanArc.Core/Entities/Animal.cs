using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArc.Core.Entites;

public partial class Animal
{
    public int AnimalId { get; set; }

    public string? Name { get; set; }

    public byte? Age { get; set; }

    public string? Type { get; set; }

    public string? Breed { get; set; }

    public string? Gender { get; set; }

    public string? Photo { get; set; }
    public bool IsAdopted { get; set; } = false;
    public string? About { get; set; }

    public string? Userid { get; set; }
    public virtual ApplicationUser? User { get; set; }

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();


}
