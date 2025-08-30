using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.Shared;
using Shared;

namespace PetFamily.Domain.PetManagment.Entities;

public class Pet : Entity<PetId>, ISoftDeletable
{
    private Pet(PetId id) : base(id) { }

    public Pet(
        PetId petId,
        string name,
        string description,
        PetColor color,
        string healthInfo,
        PetAddress petAddress,
        Phone ownerPhone,
        bool vaccinated,
        double height,
        double weight,
        PetType petType,
        DateTime createdAt,
        PetStatus petStatus = PetStatus.LookingTreatment,
        IReadOnlyCollection<Requisites>? requisites = null
        ) : base(petId)
    {
        Name = name;
        Color = color;
        OwnerPhone = ownerPhone;
        Description = description ?? string.Empty;
        HealthInfo = healthInfo;
        PetAddress = petAddress;
        Weight = weight;
        Height = height;
        Vaccinated = vaccinated;
        PetStatus = petStatus;
        PetType = petType;
        CreatedAt = createdAt;
        CreatedAt = DateTime.UtcNow;
    }

    private bool _isDeleted = false;
    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public PetColor Color;

    public string HealthInfo { get; private set; } = string.Empty;

    public PetAddress PetAddress { get; private set; } = null!;

    public double? Weight { get; private set; }

    public double? Height { get; private set; }

    public Phone OwnerPhone { get; private set; } = null!;

    public bool? Castrated { get; private set; }

    public bool Vaccinated { get; private set; }

    public DateOnly? Birthday { get; private set; }

    public PetStatus PetStatus { get; private set; } = PetStatus.LookingTreatment;

    public DateTime CreatedAt { get; private set; }

    public PetType PetType { get; private set; } = null!;


    private readonly List<Requisites> _petRequisites = [];

    public IReadOnlyCollection<Requisites> PetRequisites => _petRequisites.AsReadOnly();

    public Result<Requisites> AddRequisites(Requisites requisite)
    {
        if (requisite is null)
            return Errors.General.ValueIsInvalid("requisite");

        if (string.IsNullOrWhiteSpace(requisite.Title))
            return Errors.General.ValueIsEmptyOrWhiteSpace("Title");

        _petRequisites.Add(requisite);
        return requisite;
    }

    public Result RemoveRequisites(Requisites requisite)
    {
        if (requisite is null)
            return Errors.General.ValueIsInvalid("requisite");

        if (!_petRequisites.Contains(requisite))
            return Errors.General.NotFoundValue("requisite.Title");

        _petRequisites.Remove(requisite);
        return Result.Success();
    }

    public Result UpdateRequisites(Requisites oldRequisite, Requisites newRequisite)
    {
        var removeResult = RemoveRequisites(oldRequisite);
        if (removeResult.IsFailure)
            return removeResult;

        return AddRequisites(newRequisite);
    }

    public void Delete()
    {
        if (_isDeleted)
            return;
    }

    public void Restore()
    {
        if (!_isDeleted)
            return;
    }
}
