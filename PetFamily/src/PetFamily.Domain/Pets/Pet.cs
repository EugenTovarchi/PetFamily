using CSharpFunctionalExtensions;
using PetFamily.Domain.Requsites;

namespace PetFamily.Domain.Pets;

public class Pet
{
    public Guid Id { get; set; }
    public string Name { get; private set; } = null!;

    public string Description { get; private set; }  = string.Empty;

    public PetColor Color;

    public string HealthInfo { get; private set; } = string.Empty!;

    public string PetAddress { get; private set; } = string.Empty;

    public double? Weight { get; private set; }

    public double? Height { get; private set; }

    public string OwnerPhone { get; private set; } = null!;

    public bool Castrated { get; private set; }

    public DateOnly? Birthday { get; private set; }

    public bool? Vaccinated { get; private set; }

    public PetStatus PetStatus { get; private set; } = PetStatus.LookingTreatment;

    public DateTime CreatedAt { get; private set; }

    public PetType PetType { get; private set; }


    private readonly List<Requisites> _petRequisites = [];

    public IReadOnlyCollection<Requisites> PetRequisites => _petRequisites.AsReadOnly();


    private Pet() { }

    public Pet(
        string name,
        string description,
        PetColor color,
        string healthInfo,
        string ownerPhone,
        string petAddress,
        double weight,
        double height,
        bool vaccinated,
        PetStatus petStatus = PetStatus.LookingTreatment,
        IReadOnlyCollection<Requisites>? requisites = null)
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
        CreatedAt = DateTime.UtcNow;
    }

    public  Result AddRequisites(Requisites requisite)
    {
        if (requisite is null)
             return Result.Failure<Requisites>("Реквизиты не найдены!");

        if (string.IsNullOrWhiteSpace(requisite.Title))
            return Result.Failure("Название реквизита не может быть пустым");

        _petRequisites.Add(requisite);
        return Result.Success(requisite);
    }

    public Result RemoveRequisites(Requisites requisite)
    {
        if (requisite is null)
            return Result.Failure("Реквизит не может быть null");

        if (!_petRequisites.Contains(requisite))
            return Result.Failure("Реквизит не найден");

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
}
