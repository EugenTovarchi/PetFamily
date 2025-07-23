using PetFamily.Infrastructure.Configurations;

namespace PetFamily.Domain.Shared;

public sealed record Email
{
    public string Value { get; } = null!;

    private Email(string value) => Value = value;

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Errors.General.ValueIsInvalid("email");


        if (value.Length > Constants.MAX_MINOR_LENGTH)
            return Errors.General.ValueIsRequired("email");

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

