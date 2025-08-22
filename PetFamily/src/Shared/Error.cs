namespace Shared;

public record Error
{
    public string Code { get; }  
    public string Message { get; }  
    public ErrorType? Type { get; }

    private Error(string code, string message, ErrorType? type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    public static Error None = new(string.Empty, string.Empty, ErrorType.None);

    public static Error Validation(string code, string message) =>  
        new (code ?? "value.is.invalid", message, ErrorType.Validation);
    public static Error NotFound(string code, string message) =>
        new (code ?? "record.not.found", message, ErrorType.NotFound);
    public static Error Failure(string code, string message) =>
        new (code ?? "failure", message, ErrorType.Failure);
    public static Error Conflict(string code, string message) =>
        new (code ?? "value.is.conflict", message, ErrorType.Conflict);

    public Failure ToFailure() => new([this]);
}

public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Failure,
    Conflict
}
