using PetFamily.Domain.PetManagment.AggregateRoot;
using Shared;

namespace PetFamily.Application.Volunteers;

public interface IVolunteersRepository
{
    Task<Guid> Add(Volunteer volunteer, CancellationToken cancellationToken);

    Task<Result<Guid>> Save(Volunteer volunteer, CancellationToken cancellationToken);

    Task<Guid> Delete(Volunteer volunteer, CancellationToken cancellationToken);

    Task<Result<Volunteer>> GetById(Guid volunteerId, CancellationToken cancellationToken);

    Task<Result<Volunteer>> GetByName( string firstName, string lastName, string? middleName, CancellationToken cancellationToken);
}
