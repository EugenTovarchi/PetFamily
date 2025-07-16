using CSharpFunctionalExtensions;

namespace PetFamily.Domain.Pets
{
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
            if (speciesId == Guid.Empty || breedId == Guid.Empty)
                return Result.Failure<PetType>("ID не могут быть пустыми");

            return new PetType(speciesId, breedId);
        }
    }
}
