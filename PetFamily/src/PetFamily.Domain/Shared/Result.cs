namespace PetFamily.Domain.Shared;

public class Result
{
    public Error Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    protected Result(bool isSuccess, Error? error)
    {
        if (isSuccess && error is not null)
            throw new InvalidOperationException("Successful result cannot contain error");

        if (!isSuccess && error is null)
            throw new InvalidOperationException("Failed result must contain error");

        IsSuccess = isSuccess;
        Error = error;
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

    private Result(TValue? value, bool isSuccess, Error? error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess    
        ? _value!
        : throw new InvalidOperationException("Cannot access value of failed result");

    public static Result<TValue> Success(TValue value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        return new(value, true, null);
    }

    public new static Result<TValue> Failure(Error error)
    {
        if (error is null)
            throw new ArgumentNullException(nameof(error));

        return new(default, false, error);
    }

    public static implicit operator Result<TValue>(TValue value) => Success(value);
    public static implicit operator Result<TValue>(Error error) => Failure(error);
}