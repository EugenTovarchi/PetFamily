namespace Shared;

public record Envelope
{
    public object? Result { get; }
    public IEnumerable<ResponseError>? Errors { get; }
    public DateTime TimeGenerate    { get; }

    private Envelope(object? result, IEnumerable<ResponseError> errors)
    {
        Result = result;
        Errors = errors.ToList();
        TimeGenerate = DateTime.Now;
    }

    public static Envelope Ok (object? result = null) => new (result, []);
    public static Envelope Error(IEnumerable<ResponseError> errors) => new(null,  errors); 
}

public record ResponseError(string? ErrorCode, string? ErrorMessage, string? InvalidField);
