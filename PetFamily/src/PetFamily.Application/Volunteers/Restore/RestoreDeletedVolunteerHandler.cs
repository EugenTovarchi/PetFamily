using Microsoft.Extensions.Logging;
using PetFamily.Contracts.Requests;
using Shared;

namespace PetFamily.Application.Volunteers.Restore;

public class RestoreDeletedVolunteerHandler
{
    private readonly IVolunteersRepository _repository;
    private readonly ILogger<RestoreDeletedVolunteerHandler> _logger;
    public RestoreDeletedVolunteerHandler(IVolunteersRepository repository,
        ILogger<RestoreDeletedVolunteerHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(RestoreVolunteerRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
            return Errors.General.ValueIsInvalid("request");

        var volunteerResult = await _repository.GetById(
           request.Id,
            cancellationToken);

        if (!volunteerResult.IsSuccess)
        {
            _logger.LogWarning("Волонтёр {request.Id} не существует!", request.Id);

            return Errors.Volunteer.NotFound("volunteer");
        }

        var volunteer = volunteerResult.Value;
        if(volunteer.IsDeleted == false)
        {
            _logger.LogWarning("Не верный статус удаления волонтёра {request.Id}!", request.Id);

            return Errors.General.ValueIsInvalid("volunteer.IsDeleted");
        }
        volunteer.Restore();

        await _repository.Save(volunteer, cancellationToken);
        _logger.LogInformation("Волонтёр : {volunteerId} восстановлен", request.Id);

        return request.Id;
    }
}
