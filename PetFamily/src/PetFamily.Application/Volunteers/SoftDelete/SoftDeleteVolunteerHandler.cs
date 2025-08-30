using Microsoft.Extensions.Logging;
using PetFamily.Application.Volunteers.DeleteCommand;
using Shared;

namespace PetFamily.Application.Volunteers.SoftDelete;

public class SoftDeleteVolunteerHandler
{
    private readonly IVolunteersRepository _repository;
    private readonly ILogger<SoftDeleteVolunteerHandler> _logger;
    public SoftDeleteVolunteerHandler(IVolunteersRepository repository,
        ILogger<SoftDeleteVolunteerHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(DeleteVolunteerRequest request,
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
        if (volunteer.IsDeleted == true)
        {
            _logger.LogWarning("Волонтёр: {request.Id} уже отмечен как удаленный!", request.Id);

            return Errors.General.ValueIsInvalid("volunteer.IsDeleted");
        }

        volunteer.Delete();

        await _repository.Save(volunteer, cancellationToken);
        _logger.LogInformation("Волонтёр : {volunteerId} отмечен как удалён в БД", request.Id);

        return request.Id;
    }
}