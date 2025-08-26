using Shared;
using Shared.Constants;

namespace PetFamily.Domain.Shared;

public record FullName
{
    public string FirstName { get; } = string.Empty;
    public string LastName { get; } = string.Empty;
    public string? MiddleName { get; }

    private FullName(string firstName, string lastName, string? middleName = null)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        MiddleName = middleName?.Trim();
    }

    public static Result<FullName> Create(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) && firstName.Length > Constants.MAX_NAMES_LENGTH)
            return Errors.General.ValueIsEmptyOrWhiteSpace("firstName");

        if (string.IsNullOrWhiteSpace(lastName) && lastName.Length > Constants.MAX_NAMES_LENGTH)
            return Errors.General.ValueIsEmptyOrWhiteSpace("lastName");

        return new FullName(firstName, lastName);
    }

    public static Result<FullName> CreateWithMiddle(string firstName, string lastName, string middleName)
    {
        if (string.IsNullOrWhiteSpace(firstName) && firstName.Length > Constants.MAX_NAMES_LENGTH)
            return Errors.General.ValueIsEmptyOrWhiteSpace("firstName");

        if (string.IsNullOrWhiteSpace(lastName) && lastName.Length > Constants.MAX_NAMES_LENGTH)
            return Errors.General.ValueIsEmptyOrWhiteSpace("lastName");

        if (string.IsNullOrWhiteSpace(middleName) && middleName.Length > Constants.MAX_NAMES_LENGTH)
            return Errors.General.ValueIsEmptyOrWhiteSpace("middleName");

        return new FullName(firstName, lastName, middleName);
    }

    public string GetFullName => MiddleName == null
        ? $"{FirstName} {LastName}"
        : $"{FirstName} {MiddleName} {LastName}";

    // Для EF Core 
    private FullName() { }
}