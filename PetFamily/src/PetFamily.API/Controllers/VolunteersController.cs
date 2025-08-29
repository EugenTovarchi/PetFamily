using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Application.Volunteers.CreateVolunteer;
using PetFamily.Application.Volunteers.DeleteCommand;
using PetFamily.Application.Volunteers.UpdateMainInfoCommand;
using PetFamily.Application.Volunteers.UpdateRequisitesCommand;
using PetFamily.Application.Volunteers.UpdateSocialMediasCommand;
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

        var result = await handler.Handle(request, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("main-info/{id:guid}")]
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

    [HttpPut("social-medias/{id:guid}")]
    public async Task<IActionResult> UpdateSocialMedia(
        [FromRoute] Guid id,
        [FromBody] UpdateSocialMediaDto dto,
        [FromServices] UpdateSocialMediasHandler handler,
        [FromServices] IValidator<UpdateSocialMediaDto> _validator,
        [FromServices] ILogger<VolunteersController> _logger,
        CancellationToken cancellationToken)
    {
        var request = new UpdateSocialMediaRequest(id, dto);
        var validationResult = await _validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Социальные сети {request.Id} не валидны!", request.Id);

            return validationResult.ToValidationErrorResponse();
        }

        var result = await handler.Handle(request, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("requisites/{id:guid}")]
    public async Task<IActionResult> UpdateRequisites(
        [FromRoute] Guid id,
        [FromBody] UpdateRequisitesDto dto,
        [FromServices] UpdateRequisitesHandler handler,
        [FromServices] IValidator<UpdateRequisitesDto> _validator,
        [FromServices] ILogger<VolunteersController> _logger,
        CancellationToken cancellationToken)
    {
        var request = new UpdateRequisitesRequest(id, dto);
        var validationResult = await _validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Реквизиты {request.Id} не валидны!", request.Id);

            return validationResult.ToValidationErrorResponse();
        }

        var result = await handler.Handle(request, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }
}
