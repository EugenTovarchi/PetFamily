using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.Shared;
using Shared;
using System.Collections.Immutable;
using Constants = Shared.Constants.Constants;

namespace PetFamily.Domain.PetManagment.AggregateRoot;

public class Volunteer : Entity<VolunteerId>, ISoftDeletable
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

    private bool _isDeleted = false;
    public bool IsDeleted => _isDeleted;

    private DateTime? _deletionDate;
    public DateTime? DeletionDate => _deletionDate;

    public FullName VolunteerFullName { get; private set; } = null!;

    public Email Email { get; set; } = null!;

    public Phone Phone { get; set; } = null!;

    public string VolunteerInfo { get; set; } = string.Empty;

    public decimal ExperienceYears { get; set; } = decimal.Zero;

    private readonly List<VolunteerSocialMedia> _volunteerSocialMedias = [];

    public IReadOnlyCollection<VolunteerSocialMedia> VolunteerSocialMedias => _volunteerSocialMedias.ToList();


    private readonly List<Requisites> _volunteerRequisites = [];

    public IReadOnlyCollection<Requisites> Requisites => _volunteerRequisites.ToList();


    private readonly List<Pet> _pets = [];

    public IReadOnlyCollection<Pet> Pets => _pets.ToList();

    public int LookingTreatmentPets => CountPetsByStatus(PetStatus.LookingTreatment);
    public int LookingHomePets => CountPetsByStatus(PetStatus.LookingHome);
    public int HaveHomePets => CountPetsByStatus(PetStatus.HasHome);

    public Result UpdateInfo(string newVolunteernfo)
    {
        if(string.IsNullOrEmpty(newVolunteernfo) || newVolunteernfo.Length > Constants.MAX_INFO_LENGTH)
            return Errors.General.ValueIsEmptyOrWhiteSpace("newVolunteernfo");

        VolunteerInfo= newVolunteernfo;

        return Result.Success();
    }

    public Result UpdateExperienceYears(decimal newExperienceYears)
    {
        if(newExperienceYears < 0)
            return Errors.General.ValueMustBePositive("newVolunteernfo");

        ExperienceYears = newExperienceYears;

        return Result.Success();
    }
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

    public Result UpdateRequisites(IEnumerable<Requisites> requisites)
    {
        if (requisites is null)
            return Errors.General.ValueIsInvalid("requisites");

        _volunteerRequisites.Clear();

        _volunteerRequisites.AddRange(requisites);
        return Result.Success();
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

    public void UpdateMainInfo(
        FullName volunteerFullName,
        Email email,
        Phone phone,
        string volunteerInfo,
        decimal experienceYears)
    {
        VolunteerFullName = volunteerFullName;
        Email = email;
        Phone = phone;
        VolunteerInfo = volunteerInfo;
        ExperienceYears = experienceYears;
    }

    public Result UpdateSocialMedias(IEnumerable<VolunteerSocialMedia> socialMedias)
    {
        if (socialMedias is null)
            return Errors.General.ValueIsInvalid("socialMedias");
        
        _volunteerSocialMedias.Clear();

        _volunteerSocialMedias.AddRange(socialMedias);
        return Result.Success();
    }

    public void Delete()
    {
        if (_isDeleted)
            return;

        _isDeleted = true;
        foreach(var pet in _pets)
            pet.Delete();

        _deletionDate = DateTime.UtcNow;
    }

    public void Restore()
    {
        if (!_isDeleted)
            return;

        _isDeleted = false;
        foreach (var pet in _pets)
            pet.Restore();
    }
}
