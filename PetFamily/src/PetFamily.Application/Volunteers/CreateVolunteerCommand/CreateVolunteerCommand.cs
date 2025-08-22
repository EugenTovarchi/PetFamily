using PetFamily.Application.Volunteers.CreateVolunteer;
using PetFamily.Contracts.Requests;

namespace PetFamily.Application;

public record CreateVolunteerCommand (CreateVolunteerRequest Request);

