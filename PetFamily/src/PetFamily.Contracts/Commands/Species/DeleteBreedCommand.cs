namespace PetFamily.Contracts.Commands.Species;

public record DeleteBreedCommand(Guid SpeciesId, Guid BreedId);
