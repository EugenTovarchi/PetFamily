using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Contracts.Commands.Species;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using Shared;

namespace PetFamily.Application.Species.CreateSpecies;

public class CreateSpeciesHandler
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateSpeciesCommand> _validator;
    private readonly ILogger<CreateSpeciesHandler> _logger;

    public CreateSpeciesHandler(
        ISpeciesRepository speciesRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateSpeciesCommand> validator,
        ILogger<CreateSpeciesHandler> logger)
    {
        _speciesRepository = speciesRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, Failure>> Handle(
        CreateSpeciesCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command == null)
            return Errors.General.ValueIsInvalid("command").ToFailure();

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Вид с {command.SpeciesId} не валиден!", command.CreateSpeciesRequest.Title);

            return validationResult.ToErrors();
        }

        var speciesResult = await _speciesRepository.GetByTitle(
               command.CreateSpeciesRequest.Title,
                cancellationToken);

        if (speciesResult.IsSuccess)
        {
            _logger.LogWarning("Species: {command.CreateSpeciesRequest.Title} is already exist!",
                command.CreateSpeciesRequest.Title);

            return Errors.General.Duplicate("species").ToFailure();
        }

        var speciesId = SpeciesId.NewSpeciesId();
        var species = new Domain.PetManagment.AggregateRoot.Species(speciesId, command.CreateSpeciesRequest.Title);

        await _speciesRepository.Add(species, cancellationToken);
        _logger.LogInformation("Вид: {speciesId} создан", speciesId);

        return species.Id.Value;
    }
}
