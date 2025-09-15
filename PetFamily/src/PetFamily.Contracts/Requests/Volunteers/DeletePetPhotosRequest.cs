namespace PetFamily.Contracts.Requests.Volunteers;

public record DeletePetPhotosRequest(IEnumerable<string> PhotoPaths);

