using PetFamily.Contracts.Dtos;

namespace PetFamily.Contracts.Requests.Volunteers;

public record UpdateMainInfoRequest(
FullNameDto FullName,
string Phone,
string Email,
string VolunteerInfo,
decimal ExperienceYears);