using CleanArc.Core.Primitives;

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

    // One-to-One relationship: Each Animal has ONE MedicalRecord
    public virtual MedicalRecord? MedicalRecord { get; set; }
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public static class Errors
    {
        public static readonly Error NotFound = new(
            "Animal.NotFound",
            "The animal with the specified identifier was not found.");

        public static readonly Error AlreadyAdopted = new(
            "Animal.AlreadyAdopted",
            "This animal has already been adopted.");

        public static readonly Error CannotAdoptOwnAnimal = new(
            "Animal.CannotAdoptOwnAnimal",
            "You cannot adopt your own animal.");
    }

    public Result Adopt(string adopterId)
    {
        if (IsAdopted)
            return Errors.AlreadyAdopted;

        if (Userid == adopterId)
            return Errors.CannotAdoptOwnAnimal;

        IsAdopted = true;
        return Result.Success();
    }
}
