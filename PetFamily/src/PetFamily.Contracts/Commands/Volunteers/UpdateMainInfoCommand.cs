using PetFamily.Contracts.Requests.Volunteers;

namespace PetFamily.Contracts.Commands.Volunteer;

public record UpdateMainInfoCommand(
    Guid Id,
    UpdateMainInfoRequest Request);

