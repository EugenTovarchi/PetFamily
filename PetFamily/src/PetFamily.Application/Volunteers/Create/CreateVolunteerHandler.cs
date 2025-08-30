using Microsoft.Extensions.Logging;
using PetFamily.Contracts.Requests;
using PetFamily.Domain.PetManagment.AggregateRoot;
using PetFamily.Domain.Shared;
using Shared;

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerHandler
{
    private readonly IVolunteersRepository _repository;
    private readonly ILogger<CreateVolunteerHandler> _logger;
    public CreateVolunteerHandler(IVolunteersRepository repository,
        ILogger<CreateVolunteerHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateVolunteerRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
            return Errors.General.ValueIsInvalid("request");

        var existVolunteer = await _repository.GetByName(
            request.FullName.FirstName,
            request.FullName.LastName,
            request.FullName.MiddleName,
            cancellationToken);

        if (existVolunteer.IsSuccess)
        {
            _logger.LogWarning("Волонтёр: {FirstName} {LastName} уже существует!",
                request.FullName.FirstName,
                request.FullName.LastName);

            return Errors.General.Duplicate("existVolunteer");
        }

        var fullName = request.FullName.MiddleName is null
        ? FullName.Create(
        request.FullName.FirstName!,
        request.FullName.LastName)
        : FullName.CreateWithMiddle(
            request.FullName.FirstName,
            request.FullName.LastName,
            request.FullName.MiddleName);

        var volunteerId = VolunteerId.NewVolunteerId();

        var phone = Phone.Create(request.Phone).Value;

        var email = Email.Create(request.Email).Value;

        var volunteer = new Volunteer
        (
            volunteerId,
            fullName.Value,
            email,
            phone,
            request.VolunteerInfo,
            request.ExperienceYears
        );

        await _repository.Add(volunteer, cancellationToken);
        _logger.LogInformation("Волонтёр с {volunteerId} создан", volunteerId);

        return volunteer.Id.Value;
    }
}
