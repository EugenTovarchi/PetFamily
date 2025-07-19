using PetFamily.Domain.Shared;

namespace PetFamily.Domain.Volunteers;

public sealed record Email
{
    private const int MAX_MAIL_LENGTH = 40;
    public string Value { get; } = null!;

    private Email(string value) => Value = value;

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) && value.Length > MAX_MAIL_LENGTH)
            return "Email не может быть пустым и превышать 40 символов";

        return new Email(value.Trim());
    }

    public static implicit operator Email(string value)
    {
        var result = Create(value);
        if (result.IsFailure)
            ArgumentNullException.ThrowIfNull(value);

        return result.Value;
    }
}

