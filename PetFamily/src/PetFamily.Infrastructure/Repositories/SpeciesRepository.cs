using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Database;
using PetFamily.Domain.PetManagment.AggregateRoot;
using PetFamily.Domain.PetManagment.Entities;
using Shared;

namespace PetFamily.Infrastructure.Repositories;

public class SpeciesRepository : ISpeciesRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SpeciesRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Add(Species species, CancellationToken cancellationToken)
    {
        await _dbContext.Species.AddAsync(species, cancellationToken);
        await _dbContext.SaveChangesAsync();
        return species.Id;
    }

    public Guid Delete(Species species, CancellationToken cancellationToken = default)
    {
        _dbContext.Species.Remove(species);
        return species.Id;
    }

    public async Task<Result<Species, Error>> GetById(Guid speciesId, CancellationToken cancellationToken)
    {
        var species = await _dbContext.Species
            .Include(s => s.Breeds)
            .FirstOrDefaultAsync(s => s.Id == speciesId, cancellationToken);

        if (species is null)
            return Errors.General.NotFoundEntity("species");

        return species;
    }

    public async Task<Result<Species, Error>> GetByTitle(string title, CancellationToken cancellationToken)
    {
        var species = await _dbContext.Species
            .FirstOrDefaultAsync(s =>
                    s.Title == title, cancellationToken);

        if (species is null)
            return Errors.General.NotFoundEntity("species");

        return species;
    }

    public async Task<Result<Breed, Error>> GetBreedByBreedId(Guid breedId, CancellationToken cancellationToken)
    {
        var breed = await _dbContext.Species
            //.AsNoTracking()   //выскакивает ошибка при добавлении пета
        .SelectMany(s => s.Breeds)
        .FirstOrDefaultAsync(b => b.Id == breedId,
        cancellationToken);

        if (breed is null)
            return Errors.General.NotFoundEntity("breed");

        return breed;
    }

    public async Task<bool> BreedExistsInSpecies(Guid speciesId, string breedTitle, CancellationToken ct)
    {
        return await _dbContext.Species
            .AsNoTracking()
            .Where(s => s.Id == speciesId)
            .AnyAsync(s => s.Breeds.Any(b =>
                b.Title.ToLower() == breedTitle.ToLower()), ct);
    }

}
