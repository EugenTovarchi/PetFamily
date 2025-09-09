using CSharpFunctionalExtensions;
using Shared;

namespace PetFamily.Domain.Shared;

public record PhotoPath
{
    private PhotoPath(string path)
    {
        Path = path;
    }

    public string Path { get; }

    public static Result<PhotoPath,Error> Create(Guid path, string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            return Errors.General.ValueIsEmptyOrWhiteSpace("filePath.extension");
        }

        var correctExtension = extension.StartsWith('.') ? extension : $".{extension}";

        string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".tiff" };

        bool isValidExtension = false;
        foreach (var allowedExt in allowedExtensions)
        {
            if (correctExtension.ToLower() == allowedExt.ToLower())
            {
                isValidExtension = true;
                break;
            }
        }

        if (!isValidExtension)
            return Errors.General.ValueIsInvalid("filePath.extension");

        var fullPath = path + correctExtension;

        return new PhotoPath(fullPath);
    }

    public static Result<PhotoPath> Create(string fullPath)
    {
        return new PhotoPath(fullPath);
    }
}
