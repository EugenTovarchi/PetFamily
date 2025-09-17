using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Processors;
using PetFamily.Application.Volunteers.AddPet;
using PetFamily.Application.Volunteers.Create;
using PetFamily.Application.Volunteers.DeletePetPhotos;
using PetFamily.Application.Volunteers.GetPetPhotos;
using PetFamily.Application.Volunteers.HardDelete;
using PetFamily.Application.Volunteers.MovePetPosition;
using PetFamily.Application.Volunteers.Restore;
using PetFamily.Application.Volunteers.SoftDelete;
using PetFamily.Application.Volunteers.UpdateMainInfo;
using PetFamily.Application.Volunteers.UpdateRequisites;
using PetFamily.Application.Volunteers.UpdateSocialMedias;
using PetFamily.Application.Volunteers.UploadPetPhotos;
using PetFamily.Contracts.Commands.Volunteers;
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

    [HttpPut("main-info/{volunteerId:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid volunteerId,
        [FromBody] UpdateMainInfoRequest request,
        [FromServices] UpdateMainInfoHandler handler,
        [FromServices] ILogger<VolunteersController> _logger,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMainInfoCommand(volunteerId,request);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpDelete("soft/{volunteerId:guid}")]
    public async Task<IActionResult> SoftDelete(
        [FromRoute] Guid volunteerId,
        [FromServices] SoftDeleteVolunteerHandler handler,
        [FromServices] ILogger<VolunteersController> _logger,
        CancellationToken cancellationToken)
    {
        var command = new SoftDeleteVolunteerCommand(volunteerId);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpDelete("hard/{volunteerId:guid}")]
    public async Task<IActionResult> HardDelete(
        [FromRoute] Guid volunteerId,
        [FromServices] HardDeleteVolunteerHandler handler,
        [FromServices] ILogger<VolunteersController> _logger,
        CancellationToken cancellationToken)
    {
        var command = new HardDeleteVolunteerCommand(volunteerId);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("restore/{volunteerId:guid}")]
    public async Task<IActionResult> Restore(
        [FromRoute] Guid volunteerId,
        [FromServices] RestoreDeletedVolunteerHandler handler,
        [FromServices] ILogger<VolunteersController> _logger,
        CancellationToken cancellationToken)
    {
        var command = new RestoreVolunteerCommand(volunteerId);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("{volunteerId:guid}/social-medias")]
    public async Task<IActionResult> UpdateSocialMedia(
        [FromRoute] Guid volunteerId,
        [FromBody] UpdateSocialMediaRequest request,
        [FromServices] UpdateSocialMediasHandler handler,
        [FromServices] ILogger<VolunteersController> _logger,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSocialMediaCommand(volunteerId, request);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("{volunteerId:guid}/requisites")]
    public async Task<IActionResult> UpdateRequisites(
        [FromRoute] Guid volunteerId,
        [FromBody] UpdateRequisitesRequest request,
        [FromServices] UpdateRequisitesHandler handler,
        [FromServices] ILogger<VolunteersController> _logger,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRequisitesCommand(volunteerId, request);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPost("{volunteerId:guid}/pet")]
    public async Task<ActionResult> AddPet(
        [FromRoute] Guid volunteerId,
        [FromForm] AddPetRequest request,
        [FromServices] AddPetHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new AddPetCommand(
            volunteerId,
            request.PetName,
            request.Description,
            request.HealthInfo,
            request.Address, 
            request.Vaccinated,
            request.Height,
            request.Weight,
            request.SpeciesId,
            request.BreedId,
            [],
            request.Color,
            request.PetStatus);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPost("{volunteerId:guid}/pet/{petId:guid}/photos")]
    public async Task<ActionResult> UploadPhotos(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromForm] IFormFileCollection files,
        [FromServices] UploadPetPhotosHandler handler,
        CancellationToken cancellationToken)
    {
        await using var fileProcessor = new FormFileProcessor();
        var fileDtos = fileProcessor.Process(files);

        var command = new UploadPetPhotosCommand(volunteerId, petId, fileDtos);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpDelete("{volunteerId:guid}/pet/{petId:guid}/pet-photos")]
    public async Task<ActionResult> DeletePhotos(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] DeletePetPhotosRequest request,
        [FromServices] DeletePetPhotosHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new DeletePetPhotosCommand(volunteerId, petId, request);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPost("{volunteerId:guid}/pet/{petId:guid}/pet-photos")]
    public async Task<ActionResult> GetPhotos(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] GetPetPhotosRequest request,
        [FromServices] GetPetPhotosHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new GetPetPhotosCommand(volunteerId, petId, request);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPut("{volunteerId:guid}/pet/{petId:guid}/pet-position")]
    public async Task<ActionResult> MovePosition(
        [FromRoute] Guid volunteerId,
        [FromRoute] Guid petId,
        [FromBody] MovePetPositionRequest request,
        [FromServices] MovePetPositionHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new MovePetPositionCommand(volunteerId, petId, request);

        var result = await handler.Handle(command,cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }
}
