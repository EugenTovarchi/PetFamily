using PetFamily.Domain.Shared;

namespace PetFamily.API.Response;

public record Envelope
{
    public object? Result { get; }
    public string? ErrorCode { get; }
    public string? ErrorMessage { get; }
    public DateTime TimeGenerate    { get; }

    private Envelope(object? result, Error? error)
    {
        Result = result;
        ErrorCode = error?.Code;
        ErrorMessage = error?.Message;
        TimeGenerate = DateTime.Now;
    }

    public static Envelope Ok (object? result = null) => new (result, null);
    public static Envelope Error(Error error) => new(null, error); 
}
