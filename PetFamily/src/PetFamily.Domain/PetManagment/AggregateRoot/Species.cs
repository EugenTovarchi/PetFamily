using CSharpFunctionalExtensions;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using Shared;
using Result = CSharpFunctionalExtensions.Result;

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

    public UnitResult<Error> AddBreed(Breed breed)
    {
        if (breed is null)
            return Errors.General.ValueIsInvalid("breed");

        if (_breeds.Contains(breed))
            return Errors.General.Duplicate("breed");

        _breeds.Add(breed);
        return Result.Success<Error>();
    }

    public UnitResult<Error> RemoveBreed(Breed breed)
    {
        if (breed is null)
            return Errors.General.ValueIsInvalid("breed");

        if (!_breeds.Contains(breed))
            return Errors.General.NotFoundValue("breed");

        _breeds.Remove(breed);
        return Result.Success<Error>();
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
