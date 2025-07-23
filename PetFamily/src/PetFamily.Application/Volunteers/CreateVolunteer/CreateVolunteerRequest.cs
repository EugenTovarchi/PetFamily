 namespace PetFamily.Application.Volunteers.CreateVolunteer;

public record CreateVolunteerRequest (string  FirstName, string LastName, string Phone,
    string Email, string VolunteerInfo, decimal ExperienceYears); //подумать как создвать или добавлять Middle

