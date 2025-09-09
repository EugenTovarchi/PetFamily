using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Contracts.Commands.Species;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using Shared;

namespace PetFamily.Application.Species.AddBreed;

public  class AddBreedHandler
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<AddBreedCommand> _validator;
    private readonly ILogger<AddBreedHandler> _logger;

    public AddBreedHandler(
        ISpeciesRepository speciesRepository,
        IUnitOfWork unitOfWork,
        IValidator<AddBreedCommand> validator,
        ILogger<AddBreedHandler> logger)
    {
        _speciesRepository = speciesRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, Failure>> Handle(
        AddBreedCommand command,
        CancellationToken cancellationToken = default)
    {
        if(command == null)
             return Errors.General.ValueIsInvalid("command").ToFailure();

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Вид с {command.SpeciesId} не валиден!", command.SpeciesId);

            return validationResult.ToErrors();
        }

        var speciesResult = await _speciesRepository.GetById(
               command.SpeciesId,
                cancellationToken);

        if (!speciesResult.IsSuccess)
        {
            _logger.LogWarning("Вид {command.SpeciesId} не существует!", command.SpeciesId);

            return Errors.General.NotFoundEntity("species").ToFailure();
        }

        var speciesId = SpeciesId.Create(command.SpeciesId).Value;
        var species = speciesResult.Value;

        var breedExists = await _speciesRepository.BreedExistsInSpecies(
        command.SpeciesId,
        command.Request.BreedTitle,
        cancellationToken);

        if (breedExists)
        {
            return Errors.General.Duplicate("breed").ToFailure();
        }

        var breedId = BreedId.NewBreedId();
        var breedTitle = command.Request.BreedTitle;

        var breedResult = Breed.Create(breedId,breedTitle);
        if (breedResult.IsFailure)
            return breedResult.Error.ToFailure();

        species.AddBreed(breedResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return breedResult.Value.Id.Value;
    }
}
