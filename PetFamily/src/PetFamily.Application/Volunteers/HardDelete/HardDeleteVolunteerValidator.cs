using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Application.Volunteers.DeleteCommand;
using Shared;

namespace PetFamily.Application.Volunteers.HardDelete;

public class HardDeleteVolunteerValidation : AbstractValidator<DeleteVolunteerRequest>
{
    public HardDeleteVolunteerValidation()
    {
        RuleFor(d => d.Id).NotEmpty().WithError(Errors.General.NotFoundValue());
    }
}