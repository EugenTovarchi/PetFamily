using PetFamily.Domain.Shared;
using ValueObject = CSharpFunctionalExtensions.ValueObject;

namespace PetFamily.Domain.PetManagment.ValueObjects;

public class PetPhoto : ValueObject
{
    public PetPhoto(PhotoPath pathToStorage)
    {
        PathToStorage = pathToStorage;
    }

    public PhotoPath PathToStorage { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PathToStorage;
    }
}
