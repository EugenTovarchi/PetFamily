using Microsoft.EntityFrameworkCore;
using PetFamily.Application.Volunteers;
using PetFamily.Domain.Volunteers;

namespace PetFamily.Infrastructure.Repositories;

public class VolunteersEFCoreRepository : IVolunteersRepository
{
    private readonly ApplicationDbContext _dbContext;

    public VolunteersEFCoreRepository(ApplicationDbContext dbContext )
    {
        _dbContext = dbContext;
    }
    public Task<Guid> AddAsync(Volunteer volunteer, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> DeleteAsync(Guid volunteerId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Volunteer> GetByIdAsync(Guid volunteerId, CancellationToken cancellationToken)
    {
        var volunteer = await _dbContext.Volunteers
            .Include(v=>v.Pets)
            .Include(v=>v.Phone)
            .Include(v=>v.Email)
            .Include(v=>v.ExperienceYears)
            .Include(v=>v.VolunteerFullName)
            .Include(v=>v.VolunteerInfo)
            .Include(v=>v.VolunteerSocialMedias)
            .FirstOrDefaultAsync(v=>v.Id==volunteerId,cancellationToken);
           
        return volunteer;
    }

    public Task<Guid> UpdateAsync(Volunteer volunteer, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
