using CSharpFunctionalExtensions;
using Shared;

namespace PetFamily.Domain.Shared;

public sealed record Phone
{
    private const int MAX_PHONE_LENGTH = 12;
    public string Value { get; } = null!;

    private Phone(string value) => Value = value;

    public static Result<Phone,Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Errors.General.ValueIsEmptyOrWhiteSpace("phone");

        if(value.Length > MAX_PHONE_LENGTH)
            return Errors.General.ValueIsInvalid("phone");

        return new Phone(value.Trim());
    }

    public static implicit operator Phone(string value)
    {
        var result = Create(value);
        if (result.IsFailure)
            ArgumentNullException.ThrowIfNull(value);

        return result.Value;
    }
}

