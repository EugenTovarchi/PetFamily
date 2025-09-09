using PetFamily.Contracts.Dtos;

namespace PetFamily.Contracts.Requests.Volunteers;

public  record UpdateRequisitesRequest(IEnumerable<RequisitesDto>Dtos);

