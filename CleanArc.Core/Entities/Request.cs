namespace CleanArc.Core.Entites;

public partial class Request
{
    public int Reqid { get; set; }

    /// <summary>
    /// Owner of the animal (string FK to ApplicationUser.Id)
    /// </summary>
    public string Userid { get; set; } = string.Empty;

    /// <summary>
    /// User requesting adoption (string FK to ApplicationUser.Id)
    /// </summary>
    public string Useridreq { get; set; } = string.Empty;

    public int AnimalId { get; set; }

    public virtual Animal? Animal { get; set; }

    public string? Status { get; set; }
}
