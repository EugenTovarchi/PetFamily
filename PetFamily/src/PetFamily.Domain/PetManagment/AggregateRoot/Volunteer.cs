using CSharpFunctionalExtensions;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using PetFamily.Domain.Shared;
using Shared;
using Constants = Shared.Constants.Constants;
using Result = CSharpFunctionalExtensions.Result;

namespace PetFamily.Domain.PetManagment.AggregateRoot;

public class Volunteer : Shared.Entity<VolunteerId>, ISoftDeletable
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


    public UnitResult<Error> UpdateInfo(string newVolunteernfo)
    {
        if (string.IsNullOrEmpty(newVolunteernfo) || newVolunteernfo.Length > Constants.MAX_INFO_LENGTH)
            return Errors.General.ValueIsEmptyOrWhiteSpace("newVolunteernfo");

        VolunteerInfo = newVolunteernfo;

        return Result.Success<Error>();
    }

    public UnitResult<Error> UpdateExperienceYears(decimal newExperienceYears)
    {
        if (newExperienceYears < 0)
            return Errors.General.ValueMustBePositive("newVolunteernfo");

        ExperienceYears = newExperienceYears;

        return Result.Success<Error>();
    }
    public UnitResult<Error> AddPet(Pet pet)
    {
        if (pet == null)
            return Errors.General.ValueIsInvalid("pet");

        if (_pets.Contains(pet))
            return Errors.General.Duplicate("pet");

        var positionResult = Position.Create(_pets.Count + 1);
        if (positionResult.IsFailure)
            return positionResult.Error;

        pet.SetPosition(positionResult.Value);

        _pets.Add(pet);
        return Result.Success<Error>();
    }

    public UnitResult<Error> RemovePet(Pet pet)
    {
        if (pet is null)
            return Errors.General.ValueIsInvalid("pet");

        if (!_pets.Contains(pet))
            return Errors.General.NotFound(pet.Id);

        _pets.Remove(pet);
        return Result.Success<Error>();
    }

    public UnitResult<Error> EditPetInfo(Pet oldPetInfo, Pet newPetInfo)
    {
        var removeResult = RemovePet(oldPetInfo);
        if (removeResult.IsFailure)
            return removeResult;

        return AddPet(newPetInfo);
    }


    public Result<Requisites, Error> AddRequisites(Requisites requisite)
    {
        if (requisite is null)
            return Errors.General.ValueIsInvalid("requisite");

        if (string.IsNullOrWhiteSpace(requisite.Title))
            return Errors.General.ValueIsEmptyOrWhiteSpace("Title");

        _volunteerRequisites.Add(requisite);
        return requisite;
    }

    public UnitResult<Error> RemoveRequisites(Requisites requisite)
    {
        if (requisite is null)
            return Errors.General.ValueIsInvalid("requisite");

        if (!_volunteerRequisites.Contains(requisite))
            return Errors.General.NotFoundValue("requisite");

        _volunteerRequisites.Remove(requisite);
        return Result.Success<Error>();
    }

    public UnitResult<Error> UpdateRequisites(IEnumerable<Requisites> requisites)
    {
        if (requisites is null)
            return Errors.General.ValueIsInvalid("requisites");

        _volunteerRequisites.Clear();

        _volunteerRequisites.AddRange(requisites);
        return Result.Success<Error>();
    }


    public UnitResult<Error> AddSocialMedia(VolunteerSocialMedia socialMedia)
    {
        if (socialMedia is null)
            return Errors.General.ValueIsInvalid("socialMedia");

        if (_volunteerSocialMedias.Any(s => s.Title == socialMedia.Title))
            return Errors.General.Duplicate("socialMedia.Title");

        _volunteerSocialMedias.Add(socialMedia);
        return Result.Success<Error>();
    }
    public UnitResult<Error> RemoveSocialMedia(VolunteerSocialMedia socialMedia)
    {
        if (socialMedia is null)
            return Errors.General.ValueIsInvalid("socialMedia");

        if (!_volunteerSocialMedias.Contains(socialMedia))
            return Errors.General.NotFoundValue("socialMedia.Title");

        _volunteerSocialMedias.Remove(socialMedia);
        return Result.Success<Error>();
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

    public UnitResult<Error> UpdateSocialMedias(IEnumerable<VolunteerSocialMedia> socialMedias)
    {
        if (socialMedias is null)
            return Errors.General.ValueIsInvalid("socialMedias");

        _volunteerSocialMedias.Clear();

        _volunteerSocialMedias.AddRange(socialMedias);
        return Result.Success<Error>();
    }

    public Result<Pet, Error> GetPetById(PetId petId)
    {
        var pet = _pets.FirstOrDefault(p => p.Id == petId);
        if (pet == null)
            return Errors.General.NotFound(petId.Value);

        return pet;
    }

    public void Delete()
    {
        if (_isDeleted)
            return;

        _isDeleted = true;
        foreach (var pet in _pets)
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

    public UnitResult<Error> MovePet(Pet pet, Position newPosition)
    {
        //Текущая позиция которую мы хотим передвинуть
        var currentPosition = pet.Position;
        
        if(currentPosition == newPosition || _pets.Count == 1)
            return Result.Success<Error>();

        //обработка случаев, когда позиция выходит за границы 
        var settedPosition = SetNewPositionIfOutOfRange(newPosition);  
        if (settedPosition.IsFailure)
            return settedPosition.Error;

        newPosition = settedPosition.Value;

        var moveResult = MovePetBetweenPositions(newPosition, currentPosition);
        if (moveResult.IsFailure)
            return moveResult.Error;

        pet.SetPosition(newPosition);

        return Result.Success<Error>();
    }

    private UnitResult<Error> MovePetBetweenPositions(Position newPosition,Position currentPosition)
    {
        //если новая позици меньше текущей
        if(newPosition.Value < currentPosition.Value)
        {
            //выбираем позици, которые больше новой и меньше текущей и двигаем их вперед  
            var petsToMove = _pets.Where(p=>p.Position.Value >=  newPosition.Value
                                        && p.Position.Value < currentPosition.Value); 

            foreach(var petToMove in petsToMove)
            {
                var result = petToMove.MoveForward();
                if(result.IsFailure)
                    return result.Error;    
            }
        }
        else if(newPosition.Value > currentPosition.Value) // 4 -> 2 
        {
            var petsToMove = _pets.Where(p=>p.Position.Value > currentPosition.Value  //но не крайняя
                                            && p.Position.Value <= newPosition.Value);  //но не первая 

            foreach(var petToMove in petsToMove)
            {
                var result = petToMove.MoveBack();
                if(result.IsFailure)
                    return result.Error;
            }
        }
        return Result.Success<Error>();
    }

    private Result<Position,Error> SetNewPositionIfOutOfRange(Position newPosition)
    {
        if (newPosition.Value <= _pets.Count)
            return newPosition;

        var lastPostition = Position.Create(_pets.Count -1);
        if (lastPostition.IsFailure)
            return lastPostition.Error;

        return lastPostition.Value;
    } 
}
