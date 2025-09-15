using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Contracts.Commands.Volunteers;
using Shared;

namespace PetFamily.Application.Volunteers.Restore;

public class RestoreDeletedVolunteerHandler
{
    private readonly IVolunteersRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<RestoreVolunteerCommand> _validator;
    private readonly ILogger<RestoreDeletedVolunteerHandler> _logger;
    public RestoreDeletedVolunteerHandler(
        IVolunteersRepository repository,
        IValidator<RestoreVolunteerCommand> validator,
        IUnitOfWork unitOfWork,
        ILogger<RestoreDeletedVolunteerHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid,Failure>> Handle(RestoreVolunteerCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command is null)
            return Errors.General.ValueIsInvalid("command").ToFailure();

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Id волонтёра {command.Id} не валидно!", command.Id);

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
        if(volunteer.IsDeleted == false)
        {
            _logger.LogWarning("Не верный статус удаления волонтёра {command.Id}!", command.Id);

            return Errors.General.ValueIsInvalid("volunteer.IsDeleted").ToFailure();
        }
        volunteer.Restore();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Волонтёр : {volunteerId} восстановлен", command.Id);

        return command.Id;
    }
}
