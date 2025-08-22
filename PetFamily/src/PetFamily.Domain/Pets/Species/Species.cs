using Shared;
using System.Collections.ObjectModel;

namespace PetFamily.Domain.Pets.Species;

/// <summary>
/// Вид питомца
/// </summary>
public  class Species
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;    //тут точно не null ! 

    private readonly List<Breed> _breeds = new(); //почему класс а не VO со списком ?
    public ReadOnlyCollection<Breed> Breeds => _breeds.AsReadOnly();

    public Result<Breed> AddBreed(Breed breed)
    {
        if (breed is null)
            return  Errors.General.ValueIsInvalid("breed");

        if (string.IsNullOrWhiteSpace(breed.Title))
            return Errors.General.ValueIsEmptyOrWhiteSpace("breed");

        if (Title.Length> 100)
            return Errors.General.ValueIsRequired("breed");

        _breeds.Add(breed);
        return breed;
    }

    public Result RemoveBreed(Breed breed)
    {
        if (breed is null)
            return Errors.General.ValueIsInvalid("breed");

        if (!_breeds.Contains(breed))
            return Errors.General.NotFoundValue("breed");

        _breeds.Remove(breed);
        return Result.Success();
    }

    public Result EditBreed(Breed oldBreed, Breed newBreed)
    {
        var removeResult = RemoveBreed(oldBreed);
        if (removeResult.IsFailure)
            return removeResult;

        return AddBreed(newBreed);
    }
}
