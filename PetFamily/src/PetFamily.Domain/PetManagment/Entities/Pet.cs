using CSharpFunctionalExtensions;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using Shared;
using Result = CSharpFunctionalExtensions.Result;

namespace PetFamily.Domain.PetManagment.Entities;

public class Pet : Shared.Entity<PetId>, ISoftDeletable
{
    private Pet(PetId id) : base(id) { }

    public Pet(
        PetId petId,
        string name,
        string description,
        string healthInfo,
        Address petAddress,
        bool vaccinated,
        double height,
        double weight,
        PetType petType,
        DateTime createdAt,
        ValueObjectList<PetPhoto>? photos, 
        PetColor color ,
        PetStatus petStatus = PetStatus.LookingTreatment,
        IReadOnlyCollection<Requisites>? requisites = null
        ) : base(petId)
    {
        Name = name;
        Description = description ?? string.Empty;
        HealthInfo = healthInfo;
        PetAddress = petAddress;
        Weight = weight;
        Height = height;
        Vaccinated = vaccinated;
        Photos = photos ?? new ValueObjectList<PetPhoto>([]);
        PetStatus = petStatus;
        PetType = petType;
        CreatedAt = DateTime.UtcNow;
        Color = color;
    }

    private bool _isDeleted = false;
    public bool IsDeleted => _isDeleted;

    private DateTime? _deletionDate;
    public DateTime? DeletionDate => _deletionDate;

    public ValueObjectList<PetPhoto>? Photos { get; private set; } 

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public PetColor Color { get; private set; }

    public string HealthInfo { get; private set; } = string.Empty;

    public Address PetAddress { get; private set; } = null!;

    public double? Weight { get; private set; }

    public double? Height { get; private set; }

    public Phone? OwnerPhone { get; private set; } 

    public bool? Castrated { get; private set; }

    public bool Vaccinated { get; private set; }

    public DateOnly? Birthday { get; private set; }

    public PetStatus PetStatus { get; private set; } = PetStatus.LookingTreatment;

    public DateTime CreatedAt { get; private set; }

    public PetType PetType { get; private set; } = null!;


    private readonly List<Requisites> _petRequisites = [];

    public IReadOnlyCollection<Requisites> PetRequisites => _petRequisites.ToList();

    public UnitResult<Error> AddRequisites(Requisites requisite)
    {
        if (requisite is null)
            return Errors.General.ValueIsInvalid("requisite");

        if (string.IsNullOrWhiteSpace(requisite.Title))
            return Errors.General.ValueIsEmptyOrWhiteSpace("Title");

        _petRequisites.Add(requisite);
        return Result.Success<Error>();
    }

    public UnitResult<Error> RemoveRequisites(Requisites requisite)
    {
        if (requisite is null)
            return Errors.General.ValueIsInvalid("requisite");

        if (!_petRequisites.Contains(requisite))
            return Errors.General.NotFoundValue("requisite.Title");

        _petRequisites.Remove(requisite);
        return Result.Success<Error>();
    }

    public UnitResult<Error> UpdateRequisites(Requisites oldRequisite, Requisites newRequisite)
    {
        var removeResult = RemoveRequisites(oldRequisite);
        if (removeResult.IsFailure)
            return removeResult.Error;

        AddRequisites(newRequisite);

        return Result.Success<Error>();
    }

    public void Delete()
    {
        if (_isDeleted == true)
            return;

        _isDeleted = true;

        _deletionDate = DateTime.UtcNow;
    }

    public void Restore()
    {
        if (!_isDeleted)
            return;

        _isDeleted = false;
    }

    public void UpdatePhotos(ValueObjectList<PetPhoto> photos) =>
        Photos = photos;

    public UnitResult<Error> RemovePhotos(IEnumerable<string> deletedPaths)
    {
        if (deletedPaths is null || !deletedPaths.Any())
            return Errors.General.ValueIsEmptyOrWhiteSpace(nameof(deletedPaths));

        if (Photos is null || !Photos.Any())
            return Result.Success<Error>(); 

        var remainingPhotos = Photos
            .Where(photo => !deletedPaths.Contains(photo.PathToStorage.Path))
            .ToList();

        Photos = new ValueObjectList<PetPhoto>(remainingPhotos);

        return Result.Success<Error>();
    }
}
