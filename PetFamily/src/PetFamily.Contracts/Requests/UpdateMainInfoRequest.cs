using PetFamily.Contracts.Dtos;

namespace PetFamily.Contracts.Requests;

public  record UpdateMainInfoRequest(
    Guid Id,
    FullNameRequest FullName,
    string Phone,
    string Email,
    string VolunteerInfo,
    decimal ExperienceYears);

