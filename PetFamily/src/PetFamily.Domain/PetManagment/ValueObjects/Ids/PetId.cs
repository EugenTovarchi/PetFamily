namespace PetFamily.Domain.PetManagment.ValueObjects.Ids;

public record PetId
{
    private PetId(Guid value) => Value = value;

    public Guid Value { get; }

    public static PetId NewPetId() => new(Guid.NewGuid());

    public static PetId EmptyPetId() => new(Guid.Empty);

    public static PetId Create(Guid id) => new(id);

    public static implicit operator Guid(PetId petId)
    {
        ArgumentNullException.ThrowIfNull(petId);
        return petId.Value;
    }
}
