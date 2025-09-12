using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Commands.Species;

namespace PetFamily.Application.Species.CreateSpecies;

public class CreateSpeciesValidator : AbstractValidator<CreateSpeciesCommand>
{
    public CreateSpeciesValidator()
    {
        RuleFor(c => c.CreateSpeciesRequest.Title)
          .NotEmpty().WithError(Errors.General.ValueIsEmptyOrWhiteSpace("SpeciesTitle"))
          .MaximumLength(40).WithError(Errors.Validation.RecordIsInvalid("SpeciesTitle"));
    }
}
