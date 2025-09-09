using CSharpFunctionalExtensions;
using PetFamily.Domain.PetManagment.Entities;
using Shared;

namespace PetFamily.Application.Database;

public interface ISpeciesRepository
{
    //Task<Guid> Add(Breed breed, CancellationToken cancellationToken);

    Task<bool> BreedExistsInSpecies(Guid speciesId, string breedTitle, CancellationToken ct);

    Task<Result<Breed, Error>> GetBreedByBreedId(Guid breedId, CancellationToken cancellationToken);

    //Guid Delete(Breed breed, CancellationToken cancellationToken);

    Task<Result<Domain.PetManagment.AggregateRoot.Species,Error>> GetById(Guid speciesId, CancellationToken cancellationToken);

    //Task<Result<Breed, Error>> GetByTitle(string BreedTitle, CancellationToken cancellationToken);
}
