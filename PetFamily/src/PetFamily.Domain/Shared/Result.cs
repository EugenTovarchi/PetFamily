namespace PetFamily.Domain.Shared;

public class Result
{
    public Error Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error is not null)
            throw new InvalidOperationException("Successful result cannot contain error");

        if (!isSuccess && error is null)
            throw new InvalidOperationException("Failed result must contain error");

        if (!isSuccess)
        {
            Error = error ?? throw new InvalidOperationException(
                "Failed result must contain error. " +
                $"Type: {GetType().Name}, " +
                $"Error: {(error == null ? "null" : "not null")}");
        }
        else
        {
            Error = null!;
        }
        IsSuccess = isSuccess;
    }

    /// <summary>
    /// Создаем успешный результат
    /// </summary>
    /// <returns></returns>
    public static Result Success() => new(true, null!);
    public static Result Failure(Error error) => new(false, error ?? throw new ArgumentNullException(nameof(error)));
    public static implicit operator Result(Error error) => Failure(error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    private Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of failed result");

    public static Result<TValue> Success(TValue value) => new(value, true, null!);

    public new static Result<TValue> Failure(Error error)
    {
        if (error is null)
        {
            throw new ArgumentNullException(
                nameof(error),
                $"Error cannot be null for {typeof(TValue).Name} result");
        }
        return new(default, false, error);
    }

    public static implicit operator Result<TValue>(TValue value) => Success(value);
    public static implicit operator Result<TValue>(Error error) => Failure(error);
}