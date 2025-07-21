public  record FullName
{
    public string FirstName { get; } = string.Empty;
    public string LastName { get; } = string.Empty;
    public string? MiddleName { get; } 

    private FullName(string firstName, string lastName, string? middleName = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("Имя не может быть пустым", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Фамилия не может быть пустой", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        MiddleName = middleName?.Trim();
    }

    public static FullName Create(string firstName, string lastName)
        => new(firstName, lastName);

    public static FullName CreateWithMiddle(string firstName, string lastName, string middleName)
        => new(firstName, lastName, middleName);

    public string GetFullName => MiddleName == null
        ? $"{FirstName} {LastName}"
        : $"{FirstName} {MiddleName} {LastName}";

    // Для EF Core 
    private FullName() { } 
}