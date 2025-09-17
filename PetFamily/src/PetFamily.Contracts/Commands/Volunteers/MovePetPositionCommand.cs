using PetFamily.Contracts.Requests.Volunteers;

namespace PetFamily.Contracts.Commands.Volunteers;

public record MovePetPositionCommand(Guid VolunteerId, Guid PetId, MovePetPositionRequest Request);
