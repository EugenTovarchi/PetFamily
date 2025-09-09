using CSharpFunctionalExtensions;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using Shared;

namespace PetFamily.Domain.PetManagment.AggregateRoot;

/// <summary>
/// Вид питомца
/// </summary>
public  class Species : Shared.Entity<SpeciesId>
{
    private Species(SpeciesId id) : base(id) { }

    public Species(SpeciesId speciesId, string title) : base(speciesId)
    {   
        Title = title;
    }

    public string Title { get; set; } = null!;

    private readonly List<Breed> _breeds = [];
    public IReadOnlyCollection<Breed> Breeds => _breeds.ToList();

    public Result<Breed, Error> AddBreed(Breed breed)
    {
        if (breed is null)
            return Errors.General.ValueIsInvalid("breed");

        if (string.IsNullOrWhiteSpace(breed.Title))
            return Errors.General.ValueIsEmptyOrWhiteSpace("breed");

      
        var breedId = BreedId.NewBreedId();
        var breedResult = Breed.Create(breedId, breed.Title);

        if (breedResult.IsFailure)
            return breedResult.Error;

        _breeds.Add(breedResult.Value);
        return breedResult.Value;
    }

    public UnitResult<Error> RemoveBreed(Breed breed)
    {
        if (breed is null)
            return Errors.General.ValueIsInvalid("breed");

        if (!_breeds.Contains(breed))
            return Errors.General.NotFoundValue("breed");

        _breeds.Remove(breed);
        return CSharpFunctionalExtensions.Result.Success<Error>();
    }

    public Result<Breed, Error> EditBreed(Breed oldBreed, Breed newBreed)
    {
        var removeResult = RemoveBreed(oldBreed);
        if (removeResult.IsFailure)
            return removeResult.Error;

        return AddBreed(newBreed);
    }

    public Result<Species, Error> UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
            return Errors.General.ValueIsEmptyOrWhiteSpace("species title");

        if (newTitle.Length > 100)
            return Errors.General.ValueIsInvalid("newTitle");

        Title = newTitle;
        return this;
    }
}
