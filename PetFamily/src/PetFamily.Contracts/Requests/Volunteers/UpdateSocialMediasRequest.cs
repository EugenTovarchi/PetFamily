using PetFamily.Contracts.Dtos;

namespace PetFamily.Contracts.Requests.Volunteers;

public record UpdateSocialMediaRequest(IEnumerable<VolunteerSocialMediaDto> Dtos);
