namespace CleanArc.Application.Contracts.Responses.MedicalRecord;

public class MedicalRecordResponse
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public double Weight { get; set; }
    public double Height { get; set; }
    public string BloodType { get; set; } = string.Empty;
    public string MedicalHistoryNotes { get; set; } = string.Empty;
    public string? Injuries { get; set; }
    public string? Status { get; set; }
    public List<VaccinationResponse> Vaccinations { get; set; } = new();
}

public class VaccinationResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime DateGiven { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsExpired { get; set; }
}
