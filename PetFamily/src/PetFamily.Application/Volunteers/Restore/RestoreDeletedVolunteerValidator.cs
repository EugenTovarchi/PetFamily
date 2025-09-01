using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Requests;
using Shared;

namespace PetFamily.Application.Volunteers.Restore;

public class RestoreDeletedVolunteerValidator : AbstractValidator<RestoreVolunteerRequest>
{
    public RestoreDeletedVolunteerValidator()
    {
        RuleFor(d => d.Id).NotEmpty().WithError(Errors.General.NotFoundValue());
    }
}

