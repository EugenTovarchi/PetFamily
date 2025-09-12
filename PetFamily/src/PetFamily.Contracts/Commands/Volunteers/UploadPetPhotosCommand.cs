using PetFamily.Contracts.Dtos.FileProviderData;

namespace PetFamily.Contracts.Commands.Volunteers;

public record UploadPetPhotosCommand(Guid VolunteerId, Guid PetId, IEnumerable<UploadFileDto> Files);

