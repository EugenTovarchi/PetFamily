using PetFamily.Contracts.Requests.Species;

namespace PetFamily.Contracts.Commands.Species;

public record AddBreedCommand(Guid SpeciesId, AddBreedRequest Request);

