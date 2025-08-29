namespace PetFamily.Contracts.Dtos;

public record UpdateMainInfoDto(
FullNameDto FullName,
string Phone,
string Email,
string VolunteerInfo,
decimal ExperienceYears);