using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Contracts.Commands.Volunteers;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.Extensions;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using Shared;

namespace PetFamily.Application.Volunteers.AddPet;

public class AddPetHandler
{
    private readonly IVolunteersRepository _volunteerRepository;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<AddPetCommand> _validator;  

    private readonly ILogger<AddPetHandler> _logger;

    public AddPetHandler(
        IVolunteersRepository volunteerRepository,
        ISpeciesRepository speciesRepository,
        IUnitOfWork unitOfWork,
        IValidator<AddPetCommand> validator,
        ILogger<AddPetHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _speciesRepository = speciesRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, Failure>> Handle(
        AddPetCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var volunteerResult = await _volunteerRepository.GetById(
           command.VolunteerId,
            cancellationToken);

        if (!volunteerResult.IsSuccess)
        {
            _logger.LogWarning("Волонтёр {request.Id} не существует!", command.VolunteerId);

            return Errors.Volunteer.NotFound("volunteer").ToFailure();
        }

        var petTypeResult = await CreatePetType(command.SpeciesId, command.BreedId, cancellationToken);
        if (petTypeResult.IsFailure)
            return petTypeResult.Error.ToFailure();

        var pet = CreatePet(command, petTypeResult.Value);

        volunteerResult.Value.AddPet(pet);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return pet.Id.Value;
    }

    private Pet CreatePet(AddPetCommand command, PetType petType)
    {
        var petId = PetId.NewPetId();
        var petStatus = EnumExtension.ParsePetStatus(command.PetStatus).Value;
        var petColor = EnumExtension.ParsePetColor(command.Color).Value;

        var address = command.Address.Flat is null
        ? Address.Create(
        command.Address.City,
        command.Address.Street,
        command.Address.House)
        : Address.CreateWithFlat(
        command.Address.City,
        command.Address.Street,
        command.Address.House,
        command.Address.Flat.Value);

        var pet = new Pet(
            petId,
            command.PetName,
            command.Description,
            command.HealthInfo,
            address.Value,
            command.Vaccinated,
            command.Height,
            command.Weight,
            petType,
            DateTime.UtcNow,
            null,
            petColor,
            petStatus
        );

        return pet;
    }

    private async Task<Result<PetType, Error>> CreatePetType(
    Guid speciesIdGuid,
    Guid breedIdGuid,
    CancellationToken cancellationToken)
    {
        var speciesId = SpeciesId.Create(speciesIdGuid);
        var speciesResult = await _speciesRepository.GetById(speciesId, cancellationToken);
        if (speciesResult.IsFailure)
            return speciesResult.Error;

        var breed = speciesResult.Value.Breeds.FirstOrDefault(b => b.Id.Value == breedIdGuid);
        if (breed is null)
            return Errors.General.NotFoundEntity("breed");

        var breedId = BreedId.Create(breedIdGuid);

        var petTypeResult = PetType.Create(speciesId, breedId);
        if (petTypeResult.IsFailure)
            return petTypeResult.Error;

        return petTypeResult.Value;
    }
}
