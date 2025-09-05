using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Domain.PetManagment.ValueObjects;
using Shared;

namespace PetFamily.Application.Volunteers.UpdateSocialMediasCommand;

public class UpdateSocialMediasHandler
{
    private readonly IVolunteersRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateSocialMediasHandler> _logger;
    public UpdateSocialMediasHandler(
        IVolunteersRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateSocialMediasHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(UpdateSocialMediaRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
            return Errors.General.ValueIsInvalid("request");

        var volunteer = await _repository.GetById(
           request.Id,
            cancellationToken);

        if (!volunteer.IsSuccess)
        {
            _logger.LogWarning("Волонтёр {request.Id} не существует!", request.Id);

            return Errors.Volunteer.NotFound("volunteer");
        }

        var socialMedias = request.SocialMedias.Dtos
            .Select(sm => VolunteerSocialMedia.Create(sm.Title, sm.Url).Value)
            .ToList();

        volunteer.Value.UpdateSocialMedias(socialMedias);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Социальные сети волонтёра : {volunteerId} обновлены ", request.Id);

        return volunteer.Value.Id.Value;
    }
}