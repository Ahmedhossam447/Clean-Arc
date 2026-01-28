namespace CleanArc.Core.Primitives;

public static class UserErrors
{
    public static readonly Error NotFound = new(
        "User.NotFound",
        "The user with the specified identifier was not found.");

    public static readonly Error NotFoundByEmail = new(
        "User.NotFoundByEmail",
        "No user exists with the specified email address.");

    public static readonly Error InvalidCredentials = new(
        "User.InvalidCredentials",
        "The provided email or password is incorrect.");

    public static readonly Error EmailAlreadyExists = new(
        "User.EmailAlreadyExists",
        "A user with this email address already exists.");

    public static readonly Error UsernameAlreadyExists = new(
        "User.UsernameAlreadyExists",
        "A user with this username already exists.");

    public static readonly Error OwnerNotFound = new(
        "User.OwnerNotFound",
        "The owner of this resource was not found.");

    public static readonly Error AdopterNotFound = new(
        "User.AdopterNotFound",
        "The adopter with the specified identifier was not found.");
}
