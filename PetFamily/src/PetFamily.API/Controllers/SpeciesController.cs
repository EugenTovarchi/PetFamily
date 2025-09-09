using Microsoft.AspNetCore.Mvc;
using PetFamily.Application.Species.AddBreed;
using PetFamily.Application.Species.DeleteBreed;
using PetFamily.Contracts.Commands.Species;
using PetFamily.Contracts.Requests.Species;
using Shared;

namespace PetFamily.API.Controllers;

public class SpeciesController : ApplicationController
{
    [HttpPost("breed/{id:guid}")]
    public async Task<IActionResult> AddBreed(
        [FromRoute] Guid speciesId,
        [FromBody] AddBreedRequest request,
        [FromServices] AddBreedHandler handler,
        [FromServices] ILogger<SpeciesController> _logger,
        CancellationToken cancellationToken)
    {
        var command = new AddBreedCommand(speciesId, request);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpDelete("breed/{id:guid}")]
    public async Task<IActionResult> DeleteBreed(
        [FromRoute] Guid speciesId,
        [FromRoute] Guid breedId,
        [FromServices] DeleteBreedHandler handler,
        [FromServices] ILogger<SpeciesController> _logger,
        CancellationToken cancellationToken)
    {
        var command = new DeleteBreedCommand(speciesId, breedId);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

}
