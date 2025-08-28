using Microsoft.Extensions.Logging;
using PetFamily.Contracts.Requests;
using PetFamily.Domain.PetManagment.ValueObjects;
using Shared;

namespace PetFamily.Application.Volunteers.UpdateRequisitesCommand;

public class UpdateRequisitesHandler
{
    private readonly IVolunteersRepository _repository;
    private readonly ILogger<UpdateRequisitesHandler> _logger;
    public UpdateRequisitesHandler(IVolunteersRepository repository,
        ILogger<UpdateRequisitesHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(UpdateRequisitesRequest request,
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

        var requisites = request.UpdateRequisitesDto.Dtos
            .Select(sm => Requisites.Create(sm.Title, sm.Instruction, sm.Value).Value)
            .ToList();

        //volunteer.Value.UpdateRequisites(requisites);
        var updateResult = volunteer.Value.UpdateRequisites(requisites);
        if (updateResult.IsFailure)
        {
            return Result<Guid>.Failure(updateResult.Error);
        }

        await _repository.Save(volunteer.Value, cancellationToken);
        _logger.LogInformation("Реквизиты волонтёра : {volunteerId} обновлены ", request.Id);

        

        return volunteer.Value.Id.Value;
    }
}