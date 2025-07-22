using PetFamily.Domain.Shared;

namespace PetFamily.Domain.Volunteers;

public  record VolunteerSocialMedia(string Title, string Url)
{
    public static Result<VolunteerSocialMedia> Create(string title, string url)
    {
        if (string.IsNullOrWhiteSpace(title))
            return "Название соцсети обязательно";

        if (string.IsNullOrWhiteSpace(url))
            return "Некорректный URL";

        return new VolunteerSocialMedia(title.Trim(), url.Trim());
    }
}


