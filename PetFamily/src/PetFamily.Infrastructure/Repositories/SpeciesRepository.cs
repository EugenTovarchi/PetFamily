using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Database;
using PetFamily.Domain.PetManagment.AggregateRoot;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using Shared;

namespace PetFamily.Infrastructure.Repositories;

public class SpeciesRepository : ISpeciesRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SpeciesRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    //public async Task<Guid> Add(Volunteer volunteer, CancellationToken cancellationToken)
    //{
    //    await _dbContext.Volunteers.AddAsync(volunteer, cancellationToken);
    //    await _dbContext.SaveChangesAsync();
    //    return volunteer.Id;
    //}

    //public Guid Delete(Volunteer volunteer, CancellationToken cancellationToken = default)
    //{
    //    _dbContext.Volunteers.Remove(volunteer);
    //    return volunteer.Id;
    //}

    public async Task<Result<Species, Error>> GetById(Guid speciesId, CancellationToken cancellationToken)
    {
        var species = await _dbContext.Species
            .Include(v => v.Breeds)
            .FirstOrDefaultAsync(v => v.Id == speciesId, cancellationToken);

        if (species is null)
            return Errors.General.NotFoundEntity("species");

        return species;
    }

    //public async Task<Result<Volunteer, Error>> GetByName(string firstName, string lastName, string? middleName, CancellationToken cancellationToken)
    //{
    //    var volunteer = await _dbContext.Volunteers
    //        .Include(v => v.Pets)
    //        .FirstOrDefaultAsync(v =>
    //                v.VolunteerFullName.FirstName == firstName &&
    //                v.VolunteerFullName.LastName == lastName &&
    //                (middleName == null || v.VolunteerFullName.MiddleName == middleName),
    //                cancellationToken);

    //    if (volunteer is null)
    //        return Errors.General.ValueIsInvalid("volunteer");

    //    return volunteer;
    //}

    public async Task<Result<Breed, Error>> GetBreedByBreedId(Guid breedId, CancellationToken cancellationToken)
    {
        var breed = await _dbContext.Species
        .SelectMany(s => s.Breeds)
        .FirstOrDefaultAsync(b => b.Id == BreedId.Create(breedId),
        cancellationToken);

        if (breed is null)
            return Errors.General.ValueIsInvalid("breed");

        return breed;
    }

    public async Task<bool> BreedExistsInSpecies(Guid speciesId, string breedTitle, CancellationToken ct)
    {
        return await _dbContext.Species
            .Where(s => s.Id == SpeciesId.Create(speciesId))
            .AnyAsync(s => s.Breeds.Any(b =>
                b.Title.ToLower() == breedTitle.ToLower()), ct);
    }

}
