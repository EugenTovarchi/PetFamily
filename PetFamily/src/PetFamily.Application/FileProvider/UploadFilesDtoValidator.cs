using FluentValidation;
using PetFamily.Application.Validation;
using PetFamily.Contracts.Dtos.FileProviderData;

namespace PetFamily.Application.FileProvider;

public class UploadFilesDtoValidator : AbstractValidator<UploadFileDto>
{
    public UploadFilesDtoValidator()
    {
        RuleFor(u => u.FileName)
            .NotEmpty().WithError(Errors.General.ValueIsRequired("FileName"))
            .MaximumLength(100).WithError(Errors.General.ValueIsTooLarge("FileName", 100))
            .Must(name => name.Contains('.')).WithError(Errors.General.ValueIsInvalid("FileName"))
            .Must(name =>
                new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" }
                    .Contains(Path.GetExtension(name).ToLower()))
            .WithError(Errors.General.ValueIsInvalid("FileExtension"));

        RuleFor(u => u.Stream)
            .NotNull().WithError(Errors.General.ValueIsRequired("FileStream"))
            .Must(stream => stream != null && stream.Length > 0)
            .WithError(Errors.General.ValueIsEmpty("FileStream"))
            .Must(stream => stream != null && stream.Length <= 10 * 1024 * 1024)
            .WithError(Errors.General.ValueIsTooLarge("FileSize", 10));
    }
}



