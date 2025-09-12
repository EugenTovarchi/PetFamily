using Microsoft.AspNetCore.Mvc;
using PetFamily.Application.Species.AddBreed;
using PetFamily.Application.Species.CreateSpecies;
using PetFamily.Application.Species.DeleteBreed;
using PetFamily.Application.Species.DeleteSpecies;
using PetFamily.Contracts.Commands.Species;
using PetFamily.Contracts.Requests.Species;
using Shared;

namespace PetFamily.API.Controllers;

public class SpeciesController : ApplicationController
{
    [HttpPost]
    public async Task<IActionResult> Create(
       [FromBody] CreateSpeciesRequest request,
       [FromServices] CreateSpeciesHandler handler,
       CancellationToken cancellationToken)
    {
        var command = new CreateSpeciesCommand(request);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpDelete("{speciesId:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid speciesId,
        [FromServices] DeleteSpeciesHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteSpeciesCommand(speciesId);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPost("{speciesId:guid}/breed")]  
    public async Task<IActionResult> AddBreed(
        [FromRoute] Guid speciesId,
        [FromBody] AddBreedRequest request,
        [FromServices] AddBreedHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new AddBreedCommand(speciesId, request);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpDelete("{speciesId:guid}/breed/{breedId:guid}")]
    public async Task<IActionResult> DeleteBreed(
        [FromRoute] Guid speciesId,
        [FromRoute] Guid breedId,
        [FromServices] DeleteBreedHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteBreedCommand(speciesId, breedId);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }
}
