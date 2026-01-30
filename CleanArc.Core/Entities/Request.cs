using CleanArc.Core.Primitives;

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

    public static class Errors
    {
        public static readonly Error NotFound = new(
            "Request.NotFound",
            "The request with the specified identifier was not found.");
        public static readonly Error InvalidOwner = new(
            "Request.InvalidOwner",
            "The request has to be to the animal owner");

        public static readonly Error CannotRequestOwnAnimal = new(
            "Request.CannotRequestOwnAnimal",
            "You cannot request to adopt your own animal.");

        public static readonly Error Unauthorized = new(
            "Request.Unauthorized",
            "You are not authorized to perform this action.");

        public static readonly Error AlreadyExists = new(
            "Request.AlreadyExists",
            "You have already submitted a request for this animal.");

        public static readonly Error AlreadyApproved = new(
            "Request.AlreadyApproved",
            "This request has already been approved.");
    }
}
