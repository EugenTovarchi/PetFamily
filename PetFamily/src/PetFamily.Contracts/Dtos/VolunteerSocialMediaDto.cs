using Shared;

namespace PetFamily.Contracts.Dtos;

public record VolunteerSocialMediaDto(string Title, string Url)
{
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            return Errors.Validation.RecordIsInvalid(nameof(Title));

        if (string.IsNullOrWhiteSpace(Url))
            return Errors.Validation.RecordIsInvalid(nameof(Url));

        return Result.Success();
    }
}

