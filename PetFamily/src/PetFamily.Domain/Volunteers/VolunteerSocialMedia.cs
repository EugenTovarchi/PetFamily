using PetFamily.Domain.Shared;

namespace PetFamily.Domain.Volunteers;

public  record VolunteerSocialMedia(string Title, string Url)
{
    public static Result<VolunteerSocialMedia> Create(string title, string url)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Errors.General.ValueIsInvalid("title");

        if (string.IsNullOrWhiteSpace(url))
            return Errors.General.ValueIsInvalid("url");

        return new VolunteerSocialMedia(title.Trim(), url.Trim());
    }
}


