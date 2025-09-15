using PetFamily.Contracts.Requests.Volunteers;

namespace PetFamily.Contracts.Commands.Volunteers;

public  record DeletePetPhotosCommand(Guid VolunteerId, Guid PetId, DeletePetPhotosRequest Request);

