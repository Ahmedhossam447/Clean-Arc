using CleanArc.Core.Primitives;
using System.ComponentModel.DataAnnotations;

namespace CleanArc.Core.Entities;

public partial class Animal
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public byte? Age { get; set; }
    public string? Type { get; set; }
    public string? Breed { get; set; }
    public string? Gender { get; set; }
    public string? Photo { get; set; }
    public bool IsAdopted { get; set; } = false;
    public string? About { get; set; }
    public string? OwnerId { get; set; }
    public byte[] RowVersion { get; set; }
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

        public static readonly Error PhotoUploadFailed = new(
            "Animal.PhotoUploadFailed",
            "Failed to upload the animal photo. Please try again.");

        public static readonly Error Unauthorized = new(
            "Animal.Unauthorized",
            "You are not authorized to perform this action on this animal.");
    }

    public Result Adopt(string adopterId)
    {
        if (IsAdopted)
            return Errors.AlreadyAdopted;

        if (OwnerId == adopterId)
            return Errors.CannotAdoptOwnAnimal;

        IsAdopted = true;
        return Result.Success();
    }
}
