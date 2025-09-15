using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Commands.Species;

namespace PetFamily.Application.Species.DeleteBreed;

public class DeleteBreedValidator : AbstractValidator<DeleteBreedCommand>
{
    public DeleteBreedValidator()
    {
        RuleFor(c => c.SpeciesId).NotEmpty().WithError(Errors.General.ValueIsEmptyOrWhiteSpace("SpeciesId"));
        RuleFor(c => c.BreedId).NotEmpty().WithError(Errors.General.ValueIsEmptyOrWhiteSpace("BreedId"));
    }
}
