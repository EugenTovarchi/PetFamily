using PetFamily.Domain.Shared;
using PetFamily.Domain.Volunteers;

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerService 
{
    public async Task <Result<Guid>> AddAsync(CreateVolunteerRequest request, CancellationToken cancellationToken = default)
    {
        var volunteerId = VolunteerId.NewVolunteerId();

        var fullNameResult = FullName.Create(request.FirstName, request.LastName);
        if (fullNameResult.IsFailure)
            return fullNameResult.Error;

        var phoneResult = Phone.Create(request.Phone);
        if (phoneResult.IsFailure)
            return phoneResult.Error;

        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return emailResult.Error;

        var volunteer = new Volunteer 
        (
            volunteerId,
            fullNameResult.Value,
            emailResult.Value,
            phoneResult.Value,
            request.VolunteerInfo,
            request.ExperienceYears
        );

      

        return volunteerId.Value;
    }

    public Task<Guid> DeleteAsync(Guid volunteerId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Volunteer> GetByIdAsync(Guid volunteerId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> UpdateAsync(Volunteer volunteer, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
