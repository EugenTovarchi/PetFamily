using PetFamily.Domain.Shared;
using PetFamily.Domain.Volunteers;

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerService
{
    private readonly IVolunteersRepository _repository;
    public CreateVolunteerService(IVolunteersRepository repository)
    {
        _repository = repository;
    }
    public async Task<Result<Guid>> Add(CreateVolunteerRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
            return Errors.General.ValueIsInvalid("request");

        if (string.IsNullOrWhiteSpace(request.FirstName))
            return Errors.General.ValueIsEmptyOrWhiteSpace("request.FirstName");

        if (string.IsNullOrWhiteSpace(request.LastName))
            return Errors.General.ValueIsEmptyOrWhiteSpace("request.LastName");

        if (string.IsNullOrWhiteSpace(request.Phone))
            return Errors.General.ValueIsEmptyOrWhiteSpace("request.Phone");

        if (string.IsNullOrWhiteSpace(request.Email))
            return Errors.General.ValueIsEmptyOrWhiteSpace("request.Email");

        if (string.IsNullOrWhiteSpace(request.VolunteerInfo))
            return Errors.General.ValueIsEmptyOrWhiteSpace("request.VolunteerInfo");

        if (request.ExperienceYears < 0)
            return Errors.Volunteer.ValueMustBePositive("request.ExperienceYears");

        var existVolunteer = await _repository.GetByName(
            request.FirstName,
            request.LastName,
            request.MiddleName,
            cancellationToken);

        if (existVolunteer.IsSuccess)
            return Errors.General.Duplicate("existVolunteer");


        var fullNameResult = request.MiddleName is null
        ? FullName.Create(request.FirstName, request.LastName)
        : FullName.CreateWithMiddle(request.FirstName, request.LastName, request.MiddleName);

        var volunteerId = VolunteerId.NewVolunteerId();

        var phoneResult = Phone.Create(request.Phone);
        if (phoneResult.IsFailure)
            return phoneResult.Error;

        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure)
            return emailResult.Error;

        if (string.IsNullOrWhiteSpace(request.VolunteerInfo))
            return Errors.General.ValueIsEmptyOrWhiteSpace("request.VolunteerInfo");

        if (request.ExperienceYears < 0)
            return Errors.Volunteer.ValueMustBePositive("request.ExperienceYears");

        var volunteer = new Volunteer
        (
            volunteerId,
            fullNameResult.Value,
            emailResult.Value,
            phoneResult.Value,
            request.VolunteerInfo,
            request.ExperienceYears
        );

        await _repository.Add(volunteer, cancellationToken);
        return volunteer.Id.Value;
    }
}
