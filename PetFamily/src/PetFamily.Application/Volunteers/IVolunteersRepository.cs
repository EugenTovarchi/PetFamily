using PetFamily.Domain.Volunteers;

namespace PetFamily.Application.Volunteers;

public interface IVolunteersRepository
{
    Task <Guid> AddAsync(Volunteer volunteer, CancellationToken cancellationToken);

    Task<Guid> UpdateAsync(Volunteer volunteer, CancellationToken cancellationToken);

    Task<Guid> DeleteAsync(Guid volunteerId, CancellationToken cancellationToken);

    Task<Volunteer> GetByIdAsync(Guid volunteerId, CancellationToken cancellationToken);

   // Task<int> GeAsync(Guid volunteerId, CancellationToken cancellationToken); // зарпашиваем вопросы юзера с статусом: Open

}
