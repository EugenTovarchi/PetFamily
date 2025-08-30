using PetFamily.Contracts.Dtos;

namespace PetFamily.Application.Volunteers.UpdateSocialMediasCommand;

public record UpdateSocialMediaRequest(
    Guid Id,
    UpdateSocialMediaDto SocialMedias);
