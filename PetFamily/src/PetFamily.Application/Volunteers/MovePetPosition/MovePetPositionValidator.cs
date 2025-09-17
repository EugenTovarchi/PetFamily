using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Commands.Volunteers;

namespace PetFamily.Application.Volunteers.MovePetPosition;

public class MovePetPositionValidator : AbstractValidator<MovePetPositionCommand>
{
    public MovePetPositionValidator()
    {
        RuleFor(m => m.VolunteerId).NotEmpty().WithError(Errors.General.NotFoundValue("VolunteerId"));
        RuleFor(m => m.PetId).NotEmpty().WithError(Errors.General.NotFoundValue("PetId"));
        RuleFor(p => p.Request.NewPosition).NotEmpty().WithError(Errors.General.NotFoundValue("newPosition"))
            .GreaterThanOrEqualTo(0).WithError(Errors.General.ValueMustBePositive("new Position"));
    }
}
