using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Commands.Volunteers;

namespace PetFamily.Application.Volunteers.HardDelete;

public class HardDeleteVolunteerValidation : AbstractValidator<HardDeleteVolunteerCommand>
{
    public HardDeleteVolunteerValidation()
    {
        RuleFor(d => d.Id).NotEmpty().WithError(Errors.General.NotFoundValue());
    }
}