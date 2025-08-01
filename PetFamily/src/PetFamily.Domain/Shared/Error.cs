namespace PetFamily.Domain.Shared;

public record Error
{
    public string Code { get; }  //машинно-читаемый идентификатор ошибки
    public string Message { get; }  //человеко-читаемое описание
    public ErrorType? Type { get; }

    private Error(string code, string message, ErrorType? type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    public static Error Validation(string code, string message) =>
        new (code, message, ErrorType.Validation);
    public static Error NotFound(string code, string message) =>
        new (code, message, ErrorType.NotFound);
    public static Error Failure(string code, string message) =>
        new (code, message, ErrorType.Failure);
    public static Error Conflict(string code, string message) =>
        new (code, message, ErrorType.Conflict);
}

public enum ErrorType
{
    Validation,
    NotFound,
    Failure,
    Conflict
}
