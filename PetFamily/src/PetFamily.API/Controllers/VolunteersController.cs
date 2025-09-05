using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Processors;
using PetFamily.Application.Volunteers.AddPet;
using PetFamily.Application.Volunteers.CreateVolunteer;
using PetFamily.Application.Volunteers.DeleteCommand;
using PetFamily.Application.Volunteers.HardDelete;
using PetFamily.Application.Volunteers.Restore;
using PetFamily.Application.Volunteers.SoftDelete;
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

    [HttpDelete("soft/{id:guid}")]
    public async Task<IActionResult> SoftDelete(
        [FromRoute] Guid id,
        [FromServices] SoftDeleteVolunteerHandler handler,
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

    [HttpDelete("hard/{id:guid}")]
    public async Task<IActionResult> HardDelete(
        [FromRoute] Guid id,
        [FromServices] HardDeleteVolunteerHandler handler,
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

    [HttpPut("restore/{id:guid}")]
    public async Task<IActionResult> Restore(
        [FromRoute] Guid id,
        [FromServices] RestoreDeletedVolunteerHandler handler,
        [FromServices] IValidator<RestoreVolunteerRequest> _validator,
        [FromServices] ILogger<VolunteersController> _logger,
        CancellationToken cancellationToken)
    {
        var request = new RestoreVolunteerRequest(id);
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Id волонтёра {request.Id} не валидно!", request.Id);

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

    [HttpPost("pet/{id:guid}")]
    public async Task<ActionResult> AddPet(
        [FromRoute] Guid id,
        [FromForm] AddPetRequest request,
        [FromServices] AddPetHandler handler,
        CancellationToken cancellationToken)
    {
        await using var fileProcessor = new FormFileProcessor();
        var fileDtos = fileProcessor.Process(request.Files);

        var command = new AddPetCommand(
            id,
            request.PetName,
            request.Description,
            request.HealthInfo,
            request.Address, 
            request.Vaccinated,
            request.Height,
            request.Weight,
            fileDtos,
            request.Color,
            request.PetStatus);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }
}
