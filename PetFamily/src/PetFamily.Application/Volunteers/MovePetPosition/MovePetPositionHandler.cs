using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Contracts.Commands.Volunteers;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using PetFamily.Domain.Shared;
using Shared;

namespace PetFamily.Application.Volunteers.MovePetPosition;

public class MovePetPositionHandler
{
    private readonly IVolunteersRepository _volunteerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<MovePetPositionCommand> _validator;
    private readonly ILogger<MovePetPositionHandler> _logger;

    public MovePetPositionHandler(
        IVolunteersRepository volunteerRepository,
        IUnitOfWork unitOfWork,
        IValidator<MovePetPositionCommand> validator,
        ILogger<MovePetPositionHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<int, Failure>> Handle(
        MovePetPositionCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Record {command} is invalid!", command);
            return validationResult.ToErrors();
        }

        var volunteerResult = await _volunteerRepository.GetById(
           command.VolunteerId,
            cancellationToken);

        if (!volunteerResult.IsSuccess)
        {
            _logger.LogWarning("Волонтёр {request.Id} не существует!", command.VolunteerId);

            return Errors.Volunteer.NotFound("volunteer").ToFailure();
        }

        var petResult = volunteerResult.Value.GetPetById(PetId.Create(command.PetId));
        if (!petResult.IsSuccess)
        {
            _logger.LogWarning("Питомец {command.PetId} не существует!", command.PetId);

            return Errors.General.NotFound(command.PetId).ToFailure();
        }

        var newPosition = Position.Create(command.Request.NewPosition);

        var moveResult = volunteerResult.Value.MovePet(petResult.Value, newPosition.Value);
        if (moveResult.IsFailure)
            return moveResult.Error.ToFailure();


        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Position of pet {pet} was  moved to: {newPosition}",
             petResult.Value.Id.Value, newPosition);

        return petResult.Value.Position.Value;
    }
}
