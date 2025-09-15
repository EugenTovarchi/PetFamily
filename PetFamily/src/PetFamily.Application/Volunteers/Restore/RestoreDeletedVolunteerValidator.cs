using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Commands.Volunteers;

namespace PetFamily.Application.Volunteers.Restore;

public class RestoreDeletedVolunteerValidator : AbstractValidator<RestoreVolunteerCommand>
{
    public RestoreDeletedVolunteerValidator()
    {
        RuleFor(d => d.Id).NotEmpty().WithError(Errors.General.NotFoundValue());
    }
}

