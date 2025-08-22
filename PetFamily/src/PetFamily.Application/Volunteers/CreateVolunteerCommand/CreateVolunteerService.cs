using FluentValidation;
using PetFamily.Contracts.Requests;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Volunteers;
using Shared;

namespace PetFamily.Application.Volunteers.CreateVolunteer;

public class CreateVolunteerService
{
    private readonly IVolunteersRepository _repository;
    private readonly IValidator<CreateVolunteerRequest>  _validator;
    public CreateVolunteerService(IVolunteersRepository repository, IValidator<CreateVolunteerRequest> validator)
    {
        _repository = repository; 
        _validator = validator;
    }
    public async Task <Result<Guid>> Handle(CreateVolunteerCommand command, CancellationToken cancellationToken = default)
    {
        if (command is null)
            return Errors.General.ValueIsInvalid("request");

        var validationResult = await _validator.ValidateAsync(command.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Errors.Validation.RecordIsInvalid("createVolunteerRequest");
        }

        var existVolunteer = await _repository.GetByName(
            command.Request.FirstName,
            command.Request.LastName,
            command.Request.MiddleName,
            cancellationToken);

        if (existVolunteer.IsSuccess)
            return Errors.General.Duplicate("existVolunteer");


        var fullNameResult = command.Request.MiddleName is null
        ? FullName.Create(command.Request.FirstName, command.Request.LastName)
        : FullName.CreateWithMiddle(command.Request.FirstName, command.Request.LastName, command.Request.MiddleName);

        var volunteerId = VolunteerId.NewVolunteerId();

        var phoneResult = Phone.Create(command.Request.Phone);
        if (phoneResult.IsFailure)
            return phoneResult.Error;

        var emailResult = Email.Create(command.Request.Email);
        if (emailResult.IsFailure)
            return emailResult.Error;

        var volunteer = new Volunteer
        (
            volunteerId,
            fullNameResult.Value,
            emailResult.Value,
            phoneResult.Value,
            command.Request.VolunteerInfo,
            command.Request.ExperienceYears
        );

        await _repository.Add(volunteer, cancellationToken);
        return volunteer.Id.Value;
    }
}
