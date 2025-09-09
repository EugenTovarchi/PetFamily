using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Contracts.Commands.Volunteers;
using PetFamily.Domain.PetManagment.ValueObjects;
using Shared;

namespace PetFamily.Application.Volunteers.UpdateRequisites;

public class UpdateRequisitesHandler
{
    private readonly IVolunteersRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateRequisitesCommand> _validator;
    private readonly ILogger<UpdateRequisitesHandler> _logger;
    public UpdateRequisitesHandler(
        IVolunteersRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateRequisitesCommand> validator,
        ILogger<UpdateRequisitesHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, Failure>> Handle(UpdateRequisitesCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command is null)
            return Errors.General.ValueIsInvalid("command").ToFailure();

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Реквизиты {command.Id} не валидны!", command.Id);

            return validationResult.ToErrors();
        }

        var volunteer = await _repository.GetById(
           command.Id,
            cancellationToken);

        if (!volunteer.IsSuccess)
        {
            _logger.LogWarning("Волонтёр {command.Id} не существует!", command.Id);

            return Errors.Volunteer.NotFound("volunteer").ToFailure();
        }

        var requisites = command.UpdateRequisitesDto.Dtos
            .Select(sm => Requisites.Create(sm.Title, sm.Instruction, sm.Value).Value)
            .ToList();

        var updateResult = volunteer.Value.UpdateRequisites(requisites);
        if (updateResult.IsFailure)
        {
            return updateResult.Error.ToFailure();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Реквизиты волонтёра : {volunteerId} обновлены ", command.Id);

        return volunteer.Value.Id.Value;
    }
}