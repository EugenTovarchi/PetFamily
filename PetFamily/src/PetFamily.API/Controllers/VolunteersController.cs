using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Application;
using PetFamily.Application.Volunteers.CreateVolunteer;
using PetFamily.Contracts.Requests;
using Shared;

namespace PetFamily.API.Controllers;


public class VolunteersController : ApplicationController
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] CreateVolunteerService service,
        [FromServices] IValidator<CreateVolunteerRequest> _validator,
        [FromBody] CreateVolunteerRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToValidationErrorResponse();
        
        var command = new CreateVolunteerCommand(request);
        var result = await service.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }
}
