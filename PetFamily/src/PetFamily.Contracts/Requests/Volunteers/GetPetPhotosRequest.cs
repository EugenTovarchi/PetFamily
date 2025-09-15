namespace PetFamily.Contracts.Requests.Volunteers;

public record GetPetPhotosRequest(IEnumerable<string> PhotosPaths);

