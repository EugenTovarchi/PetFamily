using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Commands.Species;

namespace PetFamily.Application.Species.DeleteSpecies;

public class DeleteSpeciesValidator : AbstractValidator<DeleteSpeciesCommand>
{
    public DeleteSpeciesValidator()
    {
        RuleFor(c => c.SpeciesId).NotEmpty().WithError(Errors.General.ValueIsEmptyOrWhiteSpace("SpeciesId"));
    }
}
