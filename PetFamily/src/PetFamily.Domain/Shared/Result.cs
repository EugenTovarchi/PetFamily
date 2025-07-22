namespace PetFamily.Domain.Shared;

public class Result
{
    public string? Error { get; set; }
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Result(bool isSuccess, string? error)
    {
        if (isSuccess && error is not null)
            throw new InvalidOperationException();

        if(isSuccess == false && error ==null)
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);

    public static implicit operator Result (string error) => new(false,error);
}

public class Result<TValue> : Result
{ 
    private readonly TValue _value;

    public Result(TValue value, bool isSuccess, string? error) : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value
        : throw new InvalidOperationException("The value of failure result can't be accessed.");

    public static Result <TValue> Success (TValue value) => new (value, true, null);  
    public static Result <TValue> Failure (TValue value, string error) => new(default!, false , error); 
    
    public static implicit operator Result <TValue> (TValue value ) => new (value, true, null);
    public static implicit operator Result <TValue> (string error) => new(default!, false, error);
}

