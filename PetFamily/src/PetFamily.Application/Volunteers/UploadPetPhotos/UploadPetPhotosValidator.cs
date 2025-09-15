using FluentValidation;
using PetFamily.Application.FileProvider;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Commands.Volunteers;

namespace PetFamily.Application.Volunteers.UploadPetPhotos;

public class UploadPetPhotosValidator : AbstractValidator<UploadPetPhotosCommand>
{
    public UploadPetPhotosValidator()
    {
        RuleFor(u => u.VolunteerId).NotEmpty().WithError(Errors.General.NotFoundValue());
        RuleFor(u => u.PetId).NotEmpty().WithError(Errors.General.NotFoundValue());

        RuleForEach(u => u.Files).SetValidator(new UploadFilesDtoValidator());
    }
}



