namespace Application.Common.Models;

public readonly struct Result<TValue, TError>
{
    public readonly TValue? Value;
    public readonly TError? Error;

    private Result(TValue value)
    {
        IsError = false;
        Value = value;
        Error = default;
    }

    private Result(TError error)
    {
        IsError = true;
        Error = error;
        Value = default;
    }

    public bool IsError { get; }
    public bool IsSuccess => !IsError;

    public static implicit operator Result<TValue, TError>(TValue value) => new(value);

    public static implicit operator Result<TValue, TError>(TError error) => new(error);

    public TResult Match<TResult>(Func<TValue, TResult> success, Func<TError, TResult> failure) =>
        IsSuccess ? success(Value!) : failure(Error!);
}
