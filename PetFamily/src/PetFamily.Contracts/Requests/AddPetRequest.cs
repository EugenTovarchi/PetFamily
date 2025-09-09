using Microsoft.AspNetCore.Http;
using PetFamily.Contracts.Dtos;
using PetFamily.Domain.PetManagment.ValueObjects;

namespace PetFamily.Contracts.Requests;

//public record AddPetRequest(
//    string PetName,
//    string Description,
//    string HealthInfo,
//    AddressDto Address,
//    bool Vaccinated,
//    double Height,
//    double Weight,
//    PetType PetType,
//    IFormFileCollection Files,
//    string Color,
//    string PetStatus = "LookingTreatment"
//    );

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
    IFormFileCollection Files,
    string Color,
    string PetStatus = "LookingTreatment"
    );
