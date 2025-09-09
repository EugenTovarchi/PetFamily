using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Commands.Species;

namespace PetFamily.Application.Species.AddBreed;

public class AddBreedValidator : AbstractValidator<AddBreedCommand>
{
    public AddBreedValidator()
    {
        RuleFor(c => c.SpeciesId).NotEmpty().WithError(Errors.General.ValueIsEmptyOrWhiteSpace("SpeciesId"));

        RuleFor(c => c.Request.BreedTitle)
         .NotEmpty().WithError(Errors.General.ValueIsEmptyOrWhiteSpace("BreedTitle"))
         .MaximumLength(40).WithError(Errors.Validation.RecordIsInvalid("BreedTitle"));
    }
}
