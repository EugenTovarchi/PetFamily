public  record VolunteerFullName
{
    public string FirstName { get; } = string.Empty;
    public string LastName { get; } = string.Empty;
    public string? MiddleName { get; } 

    private VolunteerFullName(string firstName, string lastName, string? middleName = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("Имя не может быть пустым", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Фамилия не может быть пустой", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        MiddleName = middleName?.Trim();
    }

    public static VolunteerFullName Create(string firstName, string lastName)
        => new(firstName, lastName);

    public static VolunteerFullName CreateWithMiddle(string firstName, string lastName, string middleName)
        => new(firstName, lastName, middleName);

    public string FullName => MiddleName == null
        ? $"{FirstName} {LastName}"
        : $"{FirstName} {MiddleName} {LastName}";

    // Для EF Core 
    private VolunteerFullName() { } 
}