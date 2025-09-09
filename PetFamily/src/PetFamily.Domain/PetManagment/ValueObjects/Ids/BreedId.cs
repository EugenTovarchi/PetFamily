namespace PetFamily.Domain.PetManagment.ValueObjects.Ids;

public record BreedId
{
    private BreedId(Guid value) => Value = value;

    public Guid Value { get; }

    public static BreedId NewBreedId() => new(Guid.NewGuid());

    public static BreedId EmptyBreedId() => new(Guid.Empty);

    public static BreedId Create(Guid id) => new(id);

    public static implicit operator Guid(BreedId breedId)
    {
        ArgumentNullException.ThrowIfNull(breedId);
        return breedId.Value;
    }
}


