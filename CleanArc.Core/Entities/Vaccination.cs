using CleanArc.Core.Primitives;

namespace CleanArc.Core.Entities;

public class Vaccination
{
    public int Id { get; set; }

    // Foreign Key pointing to the "Folder" (MedicalRecord)
    public int MedicalRecordId { get; set; }
    public MedicalRecord? MedicalRecord { get; set; }

    public string Name { get; set; } = string.Empty; // e.g. "Rabies"
    public DateTime DateGiven { get; set; }
    public DateTime ExpiryDate { get; set; }
    
    // Logical Helper
    public bool IsExpired => DateTime.UtcNow > ExpiryDate;

    public static class Errors
    {
        public static readonly Error NotFound = new(
            "Vaccination.NotFound",
            "The vaccination with the specified identifier was not found.");
    }
}
