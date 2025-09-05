using PetFamily.Contracts.Dtos.FileProviderData;

namespace PetFamily.Contracts.Requests;

public record UploadPetPhotoRequest(CreateFileDto UploadFile, Guid PetId);

