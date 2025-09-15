using PetFamily.Contracts.Dtos;

namespace PetFamily.Contracts.Requests.Volunteers;

public record AddPetRequest(
    string PetName,
    string Description,
    string HealthInfo,
    AddressDto Address,
    bool Vaccinated,
    double Height,
    double Weight,
    Guid SpeciesId,
    Guid BreedId,
    string Color,
    string PetStatus = "LookingTreatment"
    );
