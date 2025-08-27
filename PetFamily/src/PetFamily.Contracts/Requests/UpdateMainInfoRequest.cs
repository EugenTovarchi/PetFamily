using PetFamily.Contracts.Dtos;

namespace PetFamily.Contracts.Requests;

public record UpdateMainInfoRequest(
    Guid Id,
    UpdateMainInfoDto Dto);

