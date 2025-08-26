using PetFamily.Contracts.Dtos;

namespace PetFamily.Contracts.Requests;

public record CreateVolunteerRequest(
    FullNameRequest FullName,
    string Phone,
    string Email,
    string VolunteerInfo,
    decimal ExperienceYears,
    IEnumerable<VolunteerSocialMediaDto>? VolunteerSocialMediaDtos,
    IEnumerable<RequisitesDto>? RequisitesDtos);



