namespace CleanArc.Core.Primitives;

public class Result<TValue>
{
    public TValue? Value { get; }
    public Error Error { get; }
    public bool IsSuccess => Error == Error.None;
    public bool IsFailure => !IsSuccess;

    private Result(TValue? value, Error error)
    {
        Value = value;
        Error = error;
    }

    public static Result<TValue> Success(TValue value) => new(value, Error.None);
    public static Result<TValue> Failure(Error error) => new(default, error);

    public static implicit operator Result<TValue>(TValue value) => Success(value);
    public static implicit operator Result<TValue>(Error error) => Failure(error);
}

public class Result
{
    public Error Error { get; }
    public bool IsSuccess => Error == Error.None;
    public bool IsFailure => !IsSuccess;

    private Result(Error error)
    {
        Error = error;
    }

    public static Result Success() => new(Error.None);
    public static Result Failure(Error error) => new(error);

    public static implicit operator Result(Error error) => Failure(error);
}
