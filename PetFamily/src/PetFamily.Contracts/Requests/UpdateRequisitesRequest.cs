using PetFamily.Contracts.Dtos;

namespace PetFamily.Contracts.Requests;

public record UpdateRequisitesRequest(Guid Id,UpdateRequisitesDto UpdateRequisitesDto);

