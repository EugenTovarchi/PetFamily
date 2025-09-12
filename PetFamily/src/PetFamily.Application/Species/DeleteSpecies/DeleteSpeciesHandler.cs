using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Contracts.Commands.Species;
using Shared;

namespace PetFamily.Application.Species.DeleteSpecies;

public class DeleteSpeciesHandler
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteSpeciesCommand> _validator;
    private readonly ILogger<DeleteSpeciesHandler> _logger;

    public DeleteSpeciesHandler(
        ISpeciesRepository speciesRepository,
        IUnitOfWork unitOfWork,
        IValidator<DeleteSpeciesCommand> validator,
        ILogger<DeleteSpeciesHandler> logger)
    {
        _speciesRepository = speciesRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, Failure>> Handle(
        DeleteSpeciesCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command == null)
            return Errors.General.ValueIsInvalid("command").ToFailure();

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Вид с {command} не валиден!", command);

            return validationResult.ToErrors();
        }

        var speciesResult = await _speciesRepository.GetById(command.SpeciesId, cancellationToken);
        if (!speciesResult.IsSuccess)
        {
            _logger.LogWarning("Вид {command.SpeciesId} не существует!", command.SpeciesId);

            return Errors.General.NotFoundEntity("species").ToFailure();
        }

        var species = speciesResult.Value;

        _speciesRepository.Delete(species, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Вид: {command.SpeciesId}  удалён ", command.SpeciesId);

        return species.Id.Value;
    }
}
