namespace CleanArc.Core.Primitives;

public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static readonly Error Unexpected = new(
        "Error.Unexpected",
        "An unexpected error occurred.");
}
