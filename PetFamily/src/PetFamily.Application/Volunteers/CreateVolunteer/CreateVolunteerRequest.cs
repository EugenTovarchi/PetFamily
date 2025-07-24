 namespace PetFamily.Application.Volunteers.CreateVolunteer;

public record CreateVolunteerRequest (string  FirstName, string LastName, string? MiddleName, string Phone,
    string Email, string VolunteerInfo, decimal ExperienceYears); 

