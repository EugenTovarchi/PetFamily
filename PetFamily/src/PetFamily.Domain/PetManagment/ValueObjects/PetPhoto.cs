using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetManagment.ValueObjects;

public record PetPhoto
{
    public PetPhoto(PhotoPath pathToStorage)
    {
        PathToStorage = pathToStorage;
    }

    public PhotoPath PathToStorage { get; }
}
