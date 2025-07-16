using CSharpFunctionalExtensions;
using System.Collections.ObjectModel;

namespace PetFamily.Domain.Pets.Species;

/// <summary>
/// Вид питомца
/// </summary>
public  class Species
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    private readonly List<Breed> _breeds = new();
    public ReadOnlyCollection<Breed> Breeds => _breeds.AsReadOnly();

    public Result AddBreed(Breed breed)
    {
        if (breed is null)
            return Result.Failure<Breed>("Порода не найдена!");

        if (string.IsNullOrWhiteSpace(breed.Title))
            return Result.Failure("Порода не может быть пустой");

        if (Title.Length> 100)
            return Result.Failure("Название породы слишком длинное");

        _breeds.Add(breed);
        return Result.Success(breed);
    }

    public Result RemoveBreed(Breed breed)
    {
        if (breed is null)
            return Result.Failure("Порода не может быть null");

        if (!_breeds.Contains(breed))
            return Result.Failure("Порода не найдена");

        _breeds.Remove(breed);
        return Result.Success();
    }

    public Result EditSpecies(Breed oldBreed, Breed newBreed)
    {
        var removeResult = RemoveBreed(oldBreed);
        if (removeResult.IsFailure)
            return removeResult;

        return AddBreed(newBreed);
    }
}
