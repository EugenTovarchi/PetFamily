using PetFamily.Domain.Shared;
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

    public Result<Breed> AddBreed(Breed breed)
    {
        if (breed is null)
            return "Порода не найдена!";

        if (string.IsNullOrWhiteSpace(breed.Title))
            return "Порода не может быть пустой";

        if (Title.Length> 100)
            return "Название породы слишком длинное";

        _breeds.Add(breed);
        return breed;
    }

    public Result RemoveBreed(Breed breed)
    {
        if (breed is null)
            return "Порода не может быть null";

        if (!_breeds.Contains(breed))
            return "Порода не найдена";

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
