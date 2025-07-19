public record VolunteerId
{
    private VolunteerId(Guid value) => Value = value;

    public Guid Value { get; }

    public static VolunteerId NewVolunteerId() => new(Guid.NewGuid());  
    public static VolunteerId EmptyVolunteerId () => new (Guid.Empty);

    public static VolunteerId Create(Guid id) => new(id);
}
