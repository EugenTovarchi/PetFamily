using PetFamily.Domain.Pets;
using PetFamily.Domain.Requsites;
using PetFamily.Domain.Shared;
using Shared;

namespace PetFamily.Domain.Volunteers;

public class Volunteer : Entity<VolunteerId>
{
    private Volunteer(VolunteerId id) : base(id) { }

    public Volunteer(
           VolunteerId volunteerId,
           FullName volunteerFullName,
           Email email,
           Phone phone,
           string? volunteerInfo = null,
           decimal experienceYears = default)

        : base(volunteerId)
    {
        VolunteerFullName = volunteerFullName;
        Email = email;
        Phone = phone;
        VolunteerInfo = volunteerInfo ?? string.Empty;
        ExperienceYears = experienceYears;
    }
 
    public FullName VolunteerFullName { get; private set; } = null!;

    public Email Email { get; set; } = null!;

    public Phone Phone { get; set; } = null!;

    public string VolunteerInfo { get; set; } = string.Empty;

    public decimal ExperienceYears { get; set; } = decimal.Zero;

    private readonly List<VolunteerSocialMedia> _volunteerSocialMedias = [];

    public IReadOnlyCollection<VolunteerSocialMedia> VolunteerSocialMedias => _volunteerSocialMedias;


    private readonly List<Requisites> _volunteerRequisites = [];

    public IReadOnlyCollection<Requisites> Requisites => _volunteerRequisites.AsReadOnly();


    private readonly List<Pet> _pets = [];

    public IReadOnlyCollection<Pet> Pets => _pets.AsReadOnly();


    public int LookingTreatmentPets => CountPetsByStatus(PetStatus.LookingTreatment);
    public int LookingHomePets => CountPetsByStatus(PetStatus.LookingHome);
    public int HaveHomePets => CountPetsByStatus(PetStatus.HasHome);

    public Result AddPet(Pet pet)
    {
        if (pet == null)
            return Errors.General.ValueIsInvalid("pet");

        if (_pets.Contains(pet))
            return Errors.General.Duplicate("pet");

        _pets.Add(pet);
        return Result.Success();
    }

    public Result RemovePet(Pet pet)
    {
        if (pet is null)
            return Errors.General.ValueIsInvalid("pet");

        if (!_pets.Contains(pet))
            return Errors.General.NotFound(pet.Id);

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


    public Result<Requisites> AddRequisites(Requisites requisite)  
    {
        if (requisite is null)
            return Errors.General.ValueIsInvalid("requisite");

        if (string.IsNullOrWhiteSpace(requisite.Title))
            return Errors.General.ValueIsEmptyOrWhiteSpace("Title");

        _volunteerRequisites.Add(requisite);
        return requisite;
    }

    public Result RemoveRequisites(Requisites requisite)
    {
        if (requisite is null)
            return Errors.General.ValueIsInvalid("requisite");

        if (!_volunteerRequisites.Contains(requisite))
            return Errors.General.NotFoundValue("requisite");

        _volunteerRequisites.Remove(requisite);
        return Result.Success();
    }

    public Result UpdateRequisites(Requisites oldRequisite, Requisites newRequisite)
    {
        var removeResult = RemoveRequisites(oldRequisite);
        if (removeResult.IsFailure)
            return removeResult;

        return AddRequisites(newRequisite);
    }


    public Result AddSocialMedia(VolunteerSocialMedia socialMedia)
    {
        if (socialMedia is null)
            return Errors.General.ValueIsInvalid("socialMedia");

        if (_volunteerSocialMedias.Any(s => s.Title == socialMedia.Title))
            return Errors.General.Duplicate("socialMedia.Title");  

        _volunteerSocialMedias.Add(socialMedia);
        return Result.Success();
    }
    public Result RemoveSocialMedia(VolunteerSocialMedia socialMedia)
    {
        if (socialMedia is null)
            return Errors.General.ValueIsInvalid("socialMedia");

        if (!_volunteerSocialMedias.Contains(socialMedia))
            return Errors.General.NotFoundValue("socialMedia.Title");

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
