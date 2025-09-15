using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Commands.Volunteers;

namespace PetFamily.Application.Volunteers.SoftDelete;

public class SoftDeleteVolunteerValidation : AbstractValidator<SoftDeleteVolunteerCommand>
{
    public SoftDeleteVolunteerValidation()
    {
        RuleFor(d => d.Id).NotEmpty().WithError(Errors.General.NotFoundValue());
    }
}
