using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Commands.Volunteers;

namespace PetFamily.Application.Volunteers.DeletePetPhotos
{
    public class DeletePetPhotosValidator : AbstractValidator<DeletePetPhotosCommand>
    {
        public DeletePetPhotosValidator()
        {
            RuleFor(d => d.VolunteerId).NotEmpty().WithError(Errors.General.NotFoundValue("VolunteerId"));
            RuleFor(d => d.PetId).NotEmpty().WithError(Errors.General.NotFoundValue("PetId"));

            RuleFor(x => x.Request.PhotoPaths)
            .NotEmpty().WithError(Errors.Validation.RecordIsInvalid("PhotoPaths"))
            .Must(paths => paths.All(path => !string.IsNullOrWhiteSpace(path)))
            .WithError(Errors.General.ValueIsEmpty("photoPaphs"));
        }
    }
}
