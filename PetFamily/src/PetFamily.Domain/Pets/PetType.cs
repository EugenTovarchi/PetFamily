using PetFamily.Domain.Shared;

namespace PetFamily.Domain.Pets;

public record PetType
{
    public Guid SpeciesId { get; }
    public Guid BreedId { get; }

    private PetType(Guid speciesId, Guid breedId)
    {
        SpeciesId = speciesId;
        BreedId = breedId;
    }

    public static Result<PetType> Create(Guid speciesId, Guid breedId)
    {
        if (speciesId == Guid.Empty)
            return Errors.General.EmptyId(speciesId);

        if(breedId == Guid.Empty)
            return Errors.General.EmptyId(breedId);

        return new PetType(speciesId, breedId);
    }
}
