using PetFamily.Domain.Shared;
using PetFamily.Domain.Volunteers;

namespace PetFamily.Application.Volunteers;

public interface IVolunteersRepository
{
    Task<Guid> Add(Volunteer volunteer, CancellationToken cancellationToken);

    Task<Guid> Update(Volunteer volunteer, CancellationToken cancellationToken);

    Task<Guid> Delete(Guid volunteerId, CancellationToken cancellationToken);

    Task<Result<Volunteer>> GetById(Guid volunteerId, CancellationToken cancellationToken);

    Task<Result<Volunteer>> GetByName( string firstName, string lastName, string? middleName, CancellationToken cancellationToken);

    // Task<int> GeAs(Guid volunteerId, CancellationToken cancellationToken); // зарпашиваем вопросы юзера с статусом: Open

}
