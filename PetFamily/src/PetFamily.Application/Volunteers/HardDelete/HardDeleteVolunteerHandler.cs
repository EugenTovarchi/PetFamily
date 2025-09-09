using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Contracts.Commands.Volunteers;
using Shared;

namespace PetFamily.Application.Volunteers.HardDelete;

public class HardDeleteVolunteerHandler
{
    private readonly IVolunteersRepository _repository;
    private readonly IValidator<HardDeleteVolunteerCommand> _validator;
    private readonly ILogger<HardDeleteVolunteerHandler> _logger;
    public HardDeleteVolunteerHandler(
        IVolunteersRepository repository,
        IValidator<HardDeleteVolunteerCommand> validator,
        ILogger<HardDeleteVolunteerHandler> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid,Failure>> Handle(HardDeleteVolunteerCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command is null)
            return Errors.General.ValueIsInvalid("command").ToFailure();

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Волонтёр {command.Id} не валиден!", command.Id);

            return validationResult.ToErrors();
        }

        var volunteerResult = await _repository.GetById(
           command.Id,
            cancellationToken);

        if (!volunteerResult.IsSuccess)
        {
            _logger.LogWarning("Волонтёр {command.Id} не существует!", command.Id);

            return Errors.Volunteer.NotFound("volunteer").ToFailure();
        }

        var volunteer = volunteerResult.Value;

        _repository.Delete(volunteer, cancellationToken); 

        _logger.LogInformation("Волонтёр: {volunteerId} полностью удалён ", command.Id);

        return command.Id;
    }
}
