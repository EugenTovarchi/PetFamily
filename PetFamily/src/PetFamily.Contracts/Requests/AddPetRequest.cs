using Microsoft.AspNetCore.Http;
using PetFamily.Contracts.Dtos;

namespace PetFamily.Contracts.Requests;

public record AddPetRequest(
    string PetName,
    string Description,
    string HealthInfo,
    AddressDto Address,
    bool Vaccinated,
    double Height,
    double Weight,
    IFormFileCollection Files,
    string Color,
    string PetStatus = "LookingTreatment"
    );
