using PetFamily.Contracts.Requests.Volunteers;

namespace PetFamily.Contracts.Commands.Volunteers;

public record GetPetPhotosCommand(Guid VolunteerId, Guid PetId, GetPetPhotosRequest Request);
