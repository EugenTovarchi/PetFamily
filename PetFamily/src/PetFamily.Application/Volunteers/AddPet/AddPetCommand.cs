using PetFamily.Contracts.Dtos;
using PetFamily.Contracts.Dtos.FileProviderData;
using PetFamily.Domain.PetManagment.ValueObjects;

namespace PetFamily.Application.Volunteers.AddPet;

/// Данные питомца из FrontEnd для его создания
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
    IEnumerable<CreateFileDto> Files, //не будет передаваться должным образом ? 
    string Color,
    string PetStatus = "LookingTreatment");

