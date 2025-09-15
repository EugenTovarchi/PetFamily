using PetFamily.Contracts.Requests.Volunteers;

namespace PetFamily.Contracts.Commands.Volunteers;

public record UpdateMainInfoCommand(
    Guid Id,
    UpdateMainInfoRequest Request);

