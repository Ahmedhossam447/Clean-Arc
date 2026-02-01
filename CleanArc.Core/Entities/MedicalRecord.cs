using CleanArc.Core.Primitives;

namespace CleanArc.Core.Entites;

public class MedicalRecord
{
    public int Id { get; set; }

    // Parent: Animal (One-to-One)
    public int AnimalId { get; set; }
    public Animal? Animal { get; set; }

    // Children: The list of vaccinations
    public ICollection<Vaccination> Vaccinations { get; set; } = new List<Vaccination>();

    // Vital Stats
    public double Weight { get; set; }
    public double Height { get; set; }
    public string BloodType { get; set; } = string.Empty;
    public string MedicalHistoryNotes { get; set; } = string.Empty;
    
    // Legacy fields (keeping for backward compatibility if needed)
    public string? Injuries { get; set; }
    public string? Status { get; set; }

    public static class Errors
    {
        public static readonly Error NotFound = new(
            "MedicalRecord.NotFound",
            "Medical record for the specified animal was not found.");
    }
}