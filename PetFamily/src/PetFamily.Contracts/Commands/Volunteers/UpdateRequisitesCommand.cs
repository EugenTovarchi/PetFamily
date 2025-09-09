using PetFamily.Contracts.Requests.Volunteers;

namespace PetFamily.Contracts.Commands.Volunteers;

public record UpdateRequisitesCommand(Guid Id,UpdateRequisitesRequest UpdateRequisitesDto);

