using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Contracts.Commands.Volunteers;
using PetFamily.Domain.Shared;
using Shared;

namespace PetFamily.Application.Volunteers.UpdateMainInfo;

public class UpdateMainInfoHandler
{
    private readonly IVolunteersRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateMainInfoCommand> _validator;
    private readonly ILogger<UpdateMainInfoHandler> _logger;
    public UpdateMainInfoHandler(
        IVolunteersRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateMainInfoCommand> validator,
        ILogger<UpdateMainInfoHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, Failure>> Handle(UpdateMainInfoCommand command,
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

        var volunteer = await _repository.GetById(
           command.Id,
            cancellationToken);

        if (!volunteer.IsSuccess)
        {
            _logger.LogWarning("Волонтёр {command.Id} не существует!", command.Id);

            return Errors.Volunteer.NotFound("volunteer").ToFailure();
        }

        var fullName = command.Request.FullName.MiddleName is null
        ? FullName.Create(
            command.Request.FullName.FirstName,
            command.Request.FullName.LastName)
        : FullName.CreateWithMiddle(
            command.Request.FullName.FirstName,
            command.Request.FullName.LastName,
            command.Request.FullName.MiddleName);

        var volunteerId = VolunteerId.Create(command.Id);

        var phone = Phone.Create(command.Request.Phone).Value;

        var email = Email.Create(command.Request.Email).Value;

        var volunteerInfo = command.Request.VolunteerInfo;

        var experienceYears = command.Request.ExperienceYears;

        volunteer.Value.UpdateMainInfo(
            fullName.Value,
            email,
            phone,
            volunteerInfo,
            experienceYears
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Данные волонтёра : {volunteerId} обновлены ", volunteerId);

        return volunteer.Value.Id.Value;
    }
}
