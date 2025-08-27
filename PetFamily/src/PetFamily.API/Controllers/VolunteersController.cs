using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Application;
using PetFamily.Application.Volunteers.CreateVolunteer;
using PetFamily.Application.Volunteers.DeleteCommand;
using PetFamily.Application.Volunteers.UpdateMainInfoCommand;
using PetFamily.Contracts.Dtos;
using PetFamily.Contracts.Requests;
using Shared;

namespace PetFamily.API.Controllers;


public class VolunteersController : ApplicationController
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] CreateVolunteerHandler handler,
        [FromServices] IValidator<CreateVolunteerRequest> _validator,
        [FromServices] ILogger<VolunteersController> _logger,
        [FromBody] CreateVolunteerRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Волонтёр {request.FullName.FirstName}" +
                "  {request.FullName.LastName} не валиден!",
                request.FullName.FirstName,
                request.FullName.LastName);

            return validationResult.ToValidationErrorResponse();
        }

        var command = new CreateVolunteerCommand(request);
        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}/main-info")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateMainInfoDto dto,
        [FromServices] UpdateMainInfoHandler handler,
        [FromServices] IValidator<UpdateMainInfoRequest> _validator,
        [FromServices] ILogger<VolunteersController> _logger,
        CancellationToken cancellationToken)
    {
        var request = new UpdateMainInfoRequest(id, dto);
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Волонтёр {request.Id} не валиден!", request.Id);

            return validationResult.ToValidationErrorResponse();
        }

        var result = await handler.Handle(request, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        [FromServices] DeleteVolunteerHandler handler,
        [FromServices] IValidator<DeleteVolunteerRequest> _validator,
        [FromServices] ILogger<VolunteersController> _logger,
        CancellationToken cancellationToken)
    {
        var request = new DeleteVolunteerRequest(id);
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Волонтёр {request.Id} не валиден!", request.Id);

            return validationResult.ToValidationErrorResponse();
        }

        var result = await handler.Handle(request, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }
}
