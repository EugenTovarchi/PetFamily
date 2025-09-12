using PetFamily.Domain.Shared;

namespace PetFamily.Contracts.Responses;

public record PetPhotoUrlResponse(PhotoPath PhotoPath, string Url, DateTime ExpiresAt);
