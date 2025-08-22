using PetFamily.Contracts.Dtos;

namespace PetFamily.Contracts.Requests;

public record CreateVolunteerRequest(
    string  FirstName,
    string LastName,
    string? MiddleName,
    string Phone,
    string Email,
    string VolunteerInfo,
    decimal ExperienceYears,
    IEnumerable<VolunteerSocialMediaDto>? VolunteerSocialMediaDtos,
    IEnumerable<RequisitesDto>? RequisitesDtos); 

 