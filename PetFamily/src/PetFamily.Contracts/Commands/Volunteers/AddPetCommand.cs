using PetFamily.Contracts.Dtos;
using PetFamily.Contracts.Dtos.FileProviderData;

namespace PetFamily.Contracts.Commands.Volunteers;

public record AddPetCommand(
    Guid VolunteerId,
    string PetName, 
    string Description,
    string HealthInfo,
    AddressDto Address,
    bool Vaccinated,
    double Height,
    double Weight,
    Guid SpeciesId,
    Guid BreedId,
    IEnumerable<UploadFileDto> Files,  
    string Color,
    string PetStatus = "LookingTreatment");

