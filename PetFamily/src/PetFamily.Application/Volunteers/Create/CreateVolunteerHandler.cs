using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Contracts.Commands.Volunteers;
using PetFamily.Domain.PetManagment.AggregateRoot;
using PetFamily.Domain.Shared;
using Shared;

namespace PetFamily.Application.Volunteers.Create;

public class CreateVolunteerHandler
{
    private readonly IVolunteersRepository _repository;
    private readonly ILogger<CreateVolunteerHandler> _logger;
    private readonly IValidator<CreateVolunteerCommand> _validator;


    public CreateVolunteerHandler(
        IVolunteersRepository repository,
        IValidator <CreateVolunteerCommand> validator,
        ILogger<CreateVolunteerHandler> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid,Failure>> Handle(CreateVolunteerCommand command, CancellationToken cancellationToken = default)
    {
        if (command is null)
            return Errors.Validation.RecordIsInvalid("command").ToFailure();

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Волонтёр {command.FullName.FirstName}" +
                "  {command.FullName.LastName} не валиден!",
                command.FullName.FirstName,
                command.FullName.LastName);

            return validationResult.ToErrors();
        }

        var existVolunteer = await _repository.GetByName(
            command.FullName.FirstName,
            command.FullName.LastName,
            command.FullName.MiddleName,
            cancellationToken);

        if (existVolunteer.IsSuccess)
        {
            _logger.LogWarning("Волонтёр: {FirstName} {LastName} уже существует!",
                command.FullName.FirstName,
                command.FullName.LastName);

            return Errors.General.Duplicate("existVolunteer").ToFailure();
        }

        var fullName = command.FullName.MiddleName is null
        ? FullName.Create(
        command.FullName.FirstName!,
        command.FullName.LastName)
        : FullName.CreateWithMiddle(
            command.FullName.FirstName,
            command.FullName.LastName,
            command.FullName.MiddleName);

        var volunteerId = VolunteerId.NewVolunteerId();

        var phone = Phone.Create(command.Phone).Value;

        var email = Email.Create(command.Email).Value;

        var volunteer = new Volunteer
        (
            volunteerId,
            fullName.Value,
            email,
            phone,
            command.VolunteerInfo,
            command.ExperienceYears
        );

        await _repository.Add(volunteer, cancellationToken);
        _logger.LogInformation("Волонтёр с {volunteerId} создан", volunteerId);

        return volunteer.Id.Value;
    }
}
