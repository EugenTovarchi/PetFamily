using Shared;

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
        if (string.IsNullOrWhiteSpace(firstName))
            return Errors.General.ValueIsEmptyOrWhiteSpace("firstName");

        if (string.IsNullOrWhiteSpace(lastName))
            return Errors.General.ValueIsEmptyOrWhiteSpace("lastName");

        return new FullName(firstName, lastName);
    }

    public static FullName CreateWithMiddle(string firstName, string lastName, string middleName)
        => new(firstName, lastName, middleName);

    public string GetFullName => MiddleName == null
        ? $"{FirstName} {LastName}"
        : $"{FirstName} {MiddleName} {LastName}";

    // Для EF Core 
    private FullName() { }
}