using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Contracts.Commands.Species;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using Shared;

namespace PetFamily.Application.Species.DeleteBreed;

public class DeleteBreedHandler
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteBreedCommand> _validator;
    private readonly ILogger<DeleteBreedHandler> _logger;

    public DeleteBreedHandler(
        ISpeciesRepository speciesRepository,
        IUnitOfWork unitOfWork,
        IValidator<DeleteBreedCommand> validator,
        ILogger<DeleteBreedHandler> logger)
    {
        _speciesRepository = speciesRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid,Failure>>Handle(
        DeleteBreedCommand command,
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
        var breed = species.Breeds.FirstOrDefault(b => b.Id.Value == command.BreedId);
        if (breed is null)
        {
            _logger.LogWarning("Порода {command.BreedId} не существует в виде {command.SpeciesId}!",
                command.BreedId, command.SpeciesId);
            return Errors.General.NotFoundEntity("breed").ToFailure();
        }

        var removeResult = species.RemoveBreed(breed);
        if (removeResult.IsFailure)
            return removeResult.Error.ToFailure();

        _logger.LogInformation("Порода: {command.BreedId}  удалёна", command.BreedId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return breed.Id.Value;
    }
}
