using CSharpFunctionalExtensions;
using PetFamily.Domain.Pets;
using PetFamily.Domain.Requsites;

namespace PetFamily.Domain.Volunteers;

public class Volunteer
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string MiddleName { get; set; } = string.Empty;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string VolunteerInfo { get; set; } = string.Empty!;

    public decimal ExperienceYears { get; set; } = decimal.Zero;

    public string Phone { get; set; } = null!;

    private readonly List<VolunteerSocialMedia> _volunteerSocialMedias = new();

    public IReadOnlyCollection<VolunteerSocialMedia> VolunteerSocialMedias => _volunteerSocialMedias;


    private readonly List<PetRequisites> _requisites = [];

    public IReadOnlyCollection<PetRequisites> Requisites => _requisites.AsReadOnly();


    private readonly List<Pet> _pets = new();

    public IReadOnlyCollection<Pet> Pets => _pets.AsReadOnly();


    public int LookingTreatmentPets => CountPetsByStatus(PetStatus.LookingTreatment);
    public int LookingHomePets => CountPetsByStatus(PetStatus.LookingHome);
    public int HaveHomePets => CountPetsByStatus(PetStatus.HasHome);

    private Volunteer() { }

    public Volunteer(
           string firstName,
           string lastName,
           string email,
           string phone,
           string? middleName = null,
           string? volunteerInfo = null,
           decimal experienceYears = 0)
    {
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName ?? string.Empty;
        Email = email;
        Phone = phone;
        VolunteerInfo = volunteerInfo ?? string.Empty;
        ExperienceYears = experienceYears;
    }


    public Result AddPet(Pet pet)
    {
        if (pet == null)
            return Result.Failure("Питомец не может иметь значение null");

        if (_pets.Contains(pet))
            return Result.Failure("Этот питомец уже закреплён за волонтёром");

        _pets.Add(pet);
        return Result.Success();
    }

    public Result RemovePet(Pet pet)
    {
        if (pet is null)
            return Result.Failure("Реквизит не может быть null");

        if (!_pets.Contains(pet))
            return Result.Failure("Реквизит не найден");

        _pets.Remove(pet);
        return Result.Success();
    }

    public Result EditPetInfo(Pet oldPetInfo, Pet newPetInfo)
    {
        var removeResult = RemovePet(oldPetInfo);
        if (removeResult.IsFailure)
            return removeResult;

        return AddPet(newPetInfo);
    }


    public Result AddRequisite(PetRequisites requisite)  // получается дублирование кода тут и в классе Pet - это ок ? не понятна связь
    {
        if (requisite is null)
            return Result.Failure("Реквизит не может быть null");

        if (string.IsNullOrWhiteSpace(requisite.Title))
            return Result.Failure("Название реквизита не может быть пустым");

        _requisites.Add(requisite);
        return Result.Success();
    }


    public Result AddSocialMedia(VolunteerSocialMedia socialMedia)
    {
        if (socialMedia is null)
            return Result.Failure("Соцсеть не может быть null");

        if (_volunteerSocialMedias.Any(s => s.Title == socialMedia.Title))
            return Result.Failure($"Соцсеть {socialMedia.Title} уже добавлена");

        _volunteerSocialMedias.Add(socialMedia);
        return Result.Success();
    }
    public Result RemoveSocialMedia(VolunteerSocialMedia socialMedia)
    {
        if (socialMedia is null)
            return Result.Failure("Соцсеть не может быть null");

        if (!_volunteerSocialMedias.Contains(socialMedia))
            return Result.Failure("Соцсеть не найдена");

        _volunteerSocialMedias.Remove(socialMedia);
        return Result.Success();
    }

    private int CountPetsByStatus(PetStatus status)
    {
        if (_pets == null || _pets.Count == 0)
            return 0;

        return _pets.Count(p => p.PetStatus == status);
    }

}
