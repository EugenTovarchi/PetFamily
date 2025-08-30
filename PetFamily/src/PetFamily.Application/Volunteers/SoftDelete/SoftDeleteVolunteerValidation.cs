using FluentValidation;
using PetFamily.Application.Validation;
using Shared;

namespace PetFamily.Application.Volunteers.DeleteCommand;

public class SoftDeleteVolunteerValidation : AbstractValidator<DeleteVolunteerRequest>
{
    public SoftDeleteVolunteerValidation()
    {
        RuleFor(d => d.Id).NotEmpty().WithError(Errors.General.NotFoundValue());
    }
}
