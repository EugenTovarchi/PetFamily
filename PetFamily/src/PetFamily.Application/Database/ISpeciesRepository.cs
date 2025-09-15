using CSharpFunctionalExtensions;
using PetFamily.Domain.PetManagment.Entities;
using Shared;

namespace PetFamily.Application.Database;

public interface ISpeciesRepository
{
    Task<Guid> Add(Domain.PetManagment.AggregateRoot.Species species, CancellationToken cancellationToken);

    Task<bool> BreedExistsInSpecies(Guid speciesId, string breedTitle, CancellationToken ct);

    Task<Result<Breed, Error>> GetBreedByBreedId(Guid breedId, CancellationToken cancellationToken);

    Task<Result<Domain.PetManagment.AggregateRoot.Species,Error>> GetById(Guid speciesId, CancellationToken cancellationToken);

    Task<Result<Domain.PetManagment.AggregateRoot.Species, Error>> GetByTitle(string title, CancellationToken cancellationToken);

    Guid Delete(Domain.PetManagment.AggregateRoot.Species species,CancellationToken cancellationToken);
}
