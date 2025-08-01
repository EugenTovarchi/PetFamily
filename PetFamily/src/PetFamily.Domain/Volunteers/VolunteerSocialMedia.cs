using PetFamily.Domain.Shared;

namespace PetFamily.Domain.Volunteers;

public  record VolunteerSocialMedia(string Title, string Url)
{
    public static Result<VolunteerSocialMedia> Create(string title, string url)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Errors.General.ValueIsEmptyOrWhiteSpace("title");

        if (string.IsNullOrWhiteSpace(url))
            return Errors.General.ValueIsEmptyOrWhiteSpace("url");

        return new VolunteerSocialMedia(title.Trim(), url.Trim());
    }
}


