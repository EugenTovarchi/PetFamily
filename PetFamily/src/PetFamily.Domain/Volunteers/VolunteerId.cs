public record VolunteerId
{
    private VolunteerId(Guid value)
    {
        Value = value;
    }

    public VolunteerId Value { get; }

    public static VolunteerId NewVolunteerId() => new(Guid.NewGuid());  
    public static VolunteerId EmptyVolunteerId () => new VolunteerId(Guid.Empty);
}