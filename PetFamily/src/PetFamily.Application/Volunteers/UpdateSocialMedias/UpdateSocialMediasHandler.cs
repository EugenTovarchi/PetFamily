using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Contracts.Commands.Volunteers;
using PetFamily.Domain.PetManagment.ValueObjects;
using Shared;

namespace PetFamily.Application.Volunteers.UpdateSocialMediasCommand;

public class UpdateSocialMediasHandler
{
    private readonly IVolunteersRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateSocialMediaCommand> _validator;
    private readonly ILogger<UpdateSocialMediasHandler> _logger;
    public UpdateSocialMediasHandler(
        IVolunteersRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateSocialMediaCommand> validator,
        ILogger<UpdateSocialMediasHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid,Failure>> Handle(UpdateSocialMediaCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command is null)
            return Errors.General.ValueIsInvalid("command").ToFailure();

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Социальные сети {request.Id} не валидны!", command.Id);

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

        var socialMedias = command.SocialMedias.Dtos
            .Select(sm => VolunteerSocialMedia.Create(sm.Title, sm.Url).Value)
            .ToList();

        volunteer.Value.UpdateSocialMedias(socialMedias);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Социальные сети волонтёра : {volunteerId} обновлены ", command.Id);

        return volunteer.Value.Id.Value;
    }
}