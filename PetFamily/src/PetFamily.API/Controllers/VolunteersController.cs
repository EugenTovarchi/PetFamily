using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Processors;
using PetFamily.Application.Volunteers.AddPet;
using PetFamily.Application.Volunteers.CreateVolunteer;
using PetFamily.Application.Volunteers.HardDelete;
using PetFamily.Application.Volunteers.Restore;
using PetFamily.Application.Volunteers.SoftDelete;
using PetFamily.Application.Volunteers.UpdateMainInfo;
using PetFamily.Application.Volunteers.UpdateRequisites;
using PetFamily.Application.Volunteers.UpdateSocialMediasCommand;
using PetFamily.Contracts.Commands.Volunteer;
using PetFamily.Contracts.Commands.Volunteers;
using PetFamily.Contracts.Requests;
using PetFamily.Contracts.Requests.Volunteers;
using Shared;

namespace PetFamily.API.Controllers;


public class VolunteersController : ApplicationController
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] CreateVolunteerHandler handler,
        [FromServices] ILogger<VolunteersController> _logger,
        [FromBody] CreateVolunteerRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateVolunteerCommand(
            request.FullName,
            request.Phone,
            request.Email,
            request.VolunteerInfo,
            request.ExperienceYears,
            request.VolunteerSocialMediaDtos,
            request.RequisitesDtos);

        var result = await handler.Handle(command, cancellationToken); 

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("main-info/{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateMainInfoRequest request,
        [FromServices] UpdateMainInfoHandler handler,
        [FromServices] ILogger<VolunteersController> _logger,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMainInfoCommand(id,request);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpDelete("soft/{id:guid}")]
    public async Task<IActionResult> SoftDelete(
        [FromRoute] Guid id,
        [FromServices] SoftDeleteVolunteerHandler handler,
        [FromServices] ILogger<VolunteersController> _logger,
        CancellationToken cancellationToken)
    {
        var command = new SoftDeleteVolunteerCommand(id);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpDelete("hard/{id:guid}")]
    public async Task<IActionResult> HardDelete(
        [FromRoute] Guid id,
        [FromServices] HardDeleteVolunteerHandler handler,
        [FromServices] ILogger<VolunteersController> _logger,
        CancellationToken cancellationToken)
    {
        var command = new HardDeleteVolunteerCommand(id);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("restore/{id:guid}")]
    public async Task<IActionResult> Restore(
        [FromRoute] Guid id,
        [FromServices] RestoreDeletedVolunteerHandler handler,
        [FromServices] ILogger<VolunteersController> _logger,
        CancellationToken cancellationToken)
    {
        var command = new RestoreVolunteerCommand(id);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("social-medias/{id:guid}")]
    public async Task<IActionResult> UpdateSocialMedia(
        [FromRoute] Guid id,
        [FromBody] UpdateSocialMediaRequest request,
        [FromServices] UpdateSocialMediasHandler handler,
        [FromServices] ILogger<VolunteersController> _logger,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSocialMediaCommand(id, request);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("requisites/{id:guid}")]
    public async Task<IActionResult> UpdateRequisites(
        [FromRoute] Guid id,
        [FromBody] UpdateRequisitesRequest request,
        [FromServices] UpdateRequisitesHandler handler,
        [FromServices] ILogger<VolunteersController> _logger,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRequisitesCommand(id, request);

        var result = await handler.Handle(command, cancellationToken);
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
            request.SpeciesId,
            request.BreedId,
            fileDtos,
            request.Color,
            request.PetStatus);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }
}
