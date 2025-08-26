using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Volunteers;
using PetFamily.Domain.PetManagment.AggregateRoot;
using Shared;

namespace PetFamily.Infrastructure.Repositories;

public class VolunteersRepository : IVolunteersRepository
{
    private readonly ApplicationDbContext _dbContext;

    public VolunteersRepository(ApplicationDbContext dbContext )
    {
        _dbContext = dbContext;
    }
    public async Task<Guid> Add(Volunteer volunteer, CancellationToken cancellationToken)
    {
        await _dbContext.Volunteers.AddAsync(volunteer, cancellationToken);
        await _dbContext.SaveChangesAsync();
        return volunteer.Id;
    }

    public async Task<Guid> Delete(Guid volunteerId, CancellationToken cancellationToken = default)
    {
        _dbContext.Remove(volunteerId);
        await _dbContext.SaveChangesAsync();
        return volunteerId;
    }

    public async Task<Result<Volunteer>> GetById(Guid volunteerId, CancellationToken cancellationToken)
    {
        var volunteer = await _dbContext.Volunteers
            .Include(v=>v.Pets)
            .FirstOrDefaultAsync(v=>v.Id.Value == volunteerId, cancellationToken);

        if (volunteer is null)
            return Errors.General.ValueIsInvalid("volunteer");

        return volunteer;
    }

    public async Task<Result<Volunteer>> GetByName(string firstName, string lastName, string? middleName, CancellationToken cancellationToken)
    {
        var volunteer = await _dbContext.Volunteers
            .Include(v => v.Pets)
            .FirstOrDefaultAsync(v =>
                    v.VolunteerFullName.FirstName == firstName &&
                    v.VolunteerFullName.LastName == lastName &&
                    (middleName == null || v.VolunteerFullName.MiddleName == middleName),
                    cancellationToken);

        if (volunteer is null)
            return Errors.General.ValueIsInvalid("volunteer");

        return volunteer;
    }

    public async Task<Result<Guid>> Update(Volunteer volunteer, CancellationToken cancellationToken)
    {
        var existingVolunteer = await _dbContext.Volunteers
            .FirstOrDefaultAsync(v => v.Id == volunteer.Id, cancellationToken);

        if (existingVolunteer is null)
            return Errors.Volunteer.NotFound("volunteer");

        _dbContext.Volunteers.Update(volunteer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return volunteer.Id.Value;
    }
}
