namespace PetFamily.Domain.Shared;

public class Result
{
    public Error? Error { get; set; }
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Result(bool isSuccess, Error? error)
    {
        if (isSuccess && error is not null)
            throw new InvalidOperationException();

        if(isSuccess == false && error ==null)
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);

    public static Result Failure(Error error) => new(false, error);

    public static implicit operator Result(Error error) => Failure(error);
}

public class Result<TValue> : Result
{
    private readonly TValue _value;

    public Result(TValue value, bool isSuccess, Error? error) : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value
        : throw new InvalidOperationException("The value of failure result can't be accessed.");

    public static Result<TValue> Success(TValue value) => new(value, true, null);
    public static Result<TValue> Failure(TValue value, string error) => new(default!, false, Error.Failure("general.error", error));
    public new static Result<TValue> Failure(Error error) => new(default!, false, error);

    public static implicit operator Result<TValue>(TValue value) => new(value, true, null);
    public static implicit operator Result<TValue>(Error error)
    {
        if (error is null)
            return Failure(Error.Failure("null.error", "Received null error"));

        return Failure(error);
    }
}

