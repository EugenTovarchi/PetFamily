using CSharpFunctionalExtensions;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using PetFamily.Domain.Shared;
using Shared;
using Shared.Constants;

namespace PetFamily.Domain.PetManagment.Entities;

/// <summary>
/// Порода питомца
/// </summary>
public class Breed: Shared.Entity<BreedId>
{
    private Breed(BreedId id) : base(id) { }

    public Breed(
        BreedId breedId,
        string title
        ) : base(breedId)
    {
        Title = title;
    }

    public string Title { get; private set; } = null!;

    public static Result<Breed, Error> Create(BreedId id, string title)
    {
        if (id == null)
            return Errors.General.ValueIsRequired("breed_id");

        if (string.IsNullOrWhiteSpace(title))
            return Errors.General.ValueIsEmptyOrWhiteSpace("breed_title");

        if (title.Length > Constants.MAX_MINOR_LENGTH)
            return Errors.General.ValueIsInvalid("breed_title");

        return new Breed(id, title);
    }
}
