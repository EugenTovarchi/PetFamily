using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Contracts.Commands.Volunteers;
using Shared;

namespace PetFamily.Application.Volunteers.SoftDelete;

public class SoftDeleteVolunteerHandler
{
    private readonly IVolunteersRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<SoftDeleteVolunteerCommand> _validator;
    private readonly ILogger<SoftDeleteVolunteerHandler> _logger;
    public SoftDeleteVolunteerHandler(
        IVolunteersRepository repository,
        IValidator<SoftDeleteVolunteerCommand> validator,
        IUnitOfWork unitOfWork,
        ILogger<SoftDeleteVolunteerHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, Failure>> Handle(SoftDeleteVolunteerCommand command,
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
        if (volunteer.IsDeleted == true)
        {
            _logger.LogWarning("Волонтёр: {command.Id} уже отмечен как удаленный!", command.Id);

            return Errors.General.ValueIsInvalid("volunteer.IsDeleted").ToFailure();
        }

        volunteer.Delete();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Волонтёр : {volunteerId} отмечен как удалён в БД", command.Id);

        return command.Id;
    }
}