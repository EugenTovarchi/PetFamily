namespace PetFamily.Contracts.Dtos;

public record UpdateMainInfoDto(
FullNameRequest FullName,
string Phone,
string Email,
string VolunteerInfo,
decimal ExperienceYears);