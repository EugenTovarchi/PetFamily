using CSharpFunctionalExtensions;
using Shared.Constants;
using Shared;

namespace PetFamily.Domain.Shared;

public sealed record Email
{
    public string Value { get; } = null!;

    private Email(string value) => Value = value;

    public static Result<Email,Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Errors.General.ValueIsEmptyOrWhiteSpace("email");
            


        if (value.Length > Constants.MAX_MINOR_LENGTH)
            return Errors.General.ValueIsInvalid("email");

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

