using PetFamily.Contracts.Requests.Volunteers;

namespace PetFamily.Contracts.Commands.Volunteers;

public record UpdateSocialMediaCommand(
    Guid Id,
    UpdateSocialMediaRequest SocialMedias);
