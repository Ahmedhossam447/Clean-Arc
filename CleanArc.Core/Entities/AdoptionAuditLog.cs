namespace CleanArc.Core.Entites;

/// <summary>
/// Audit log entry for animal adoptions - provides compliance and historical tracking
/// </summary>
public class AdoptionAuditLog
{
    public int Id { get; set; }
    
    public int AnimalId { get; set; }
    public string AnimalName { get; set; } = string.Empty;
    public string AnimalType { get; set; } = string.Empty;
    
    public string AdopterId { get; set; } = string.Empty;
    public string AdopterName { get; set; } = string.Empty;
    public string AdopterEmail { get; set; } = string.Empty;
    
    public string PreviousOwnerId { get; set; } = string.Empty;
    public string PreviousOwnerEmail { get; set; } = string.Empty;
    
    public DateTime AdoptedAt { get; set; }
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Optional: IP address or source system that processed the adoption
    /// </summary>
    public string? ProcessedBy { get; set; }
}
