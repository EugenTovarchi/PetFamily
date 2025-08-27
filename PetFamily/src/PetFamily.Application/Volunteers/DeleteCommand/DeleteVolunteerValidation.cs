using FluentValidation;
using PetFamily.Application.Validation;
using Shared;

namespace PetFamily.Application.Volunteers.DeleteCommand;

public class DeleteVolunteerValidation : AbstractValidator<DeleteVolunteerRequest>
{
    public DeleteVolunteerValidation()
    {
        RuleFor(d => d.Id).NotEmpty().WithError(Errors.General.NotFoundValue());
    }
}
