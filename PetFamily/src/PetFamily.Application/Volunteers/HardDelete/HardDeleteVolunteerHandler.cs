using Microsoft.Extensions.Logging;
using PetFamily.Application.Volunteers.DeleteCommand;
using Shared;

namespace PetFamily.Application.Volunteers.HardDelete;

public class HardDeleteVolunteerHandler
{
    private readonly IVolunteersRepository _repository;
    private readonly ILogger<HardDeleteVolunteerHandler> _logger;
    public HardDeleteVolunteerHandler(IVolunteersRepository repository,
        ILogger<HardDeleteVolunteerHandler> logger)
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

        await _repository.Delete(volunteer, cancellationToken);
        _logger.LogInformation("Волонтёр: {volunteerId} полностью удалён ", request.Id);

        return request.Id;
    }
}
