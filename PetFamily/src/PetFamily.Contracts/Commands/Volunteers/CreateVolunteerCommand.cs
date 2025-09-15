using PetFamily.Contracts.Dtos;

namespace PetFamily.Contracts.Commands.Volunteers;

public record CreateVolunteerCommand(
    FullNameDto FullName,
    string Phone,
    string Email,
    string VolunteerInfo,
    decimal ExperienceYears,
    IEnumerable<VolunteerSocialMediaDto>? VolunteerSocialMediaDtos,
    IEnumerable<RequisitesDto>? RequisitesDtos);
