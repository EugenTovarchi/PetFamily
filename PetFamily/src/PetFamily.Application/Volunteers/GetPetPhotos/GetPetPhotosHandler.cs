using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Application.FileProvider;
using PetFamily.Contracts.Commands.Volunteers;
using PetFamily.Contracts.Responses;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using PetFamily.Domain.Shared;
using Shared;

namespace PetFamily.Application.Volunteers.GetPetPhotos;

public class GetPetPhotosHandler
{
    private const string BUCKET_NAME = "photos";

    private readonly IFileProvider _fileProvider;
    private readonly IVolunteersRepository _volunteerRepository;
    private readonly IValidator<GetPetPhotosCommand> _validator;
    private readonly ILogger<GetPetPhotosHandler> _logger;

    public GetPetPhotosHandler(
        IFileProvider fileProvider,
        IVolunteersRepository volunteerRepository,
        IValidator<GetPetPhotosCommand> validator,
        ILogger<GetPetPhotosHandler> logger)
    {
        _fileProvider = fileProvider;
        _volunteerRepository = volunteerRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<PetPhotoUrlResponse>, Failure>> Handle(
        GetPetPhotosCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Record {command} is invalid!", command);
            return validationResult.ToErrors();
        }

        var volunteerResult = await _volunteerRepository.GetById(
           command.VolunteerId,
            cancellationToken);

        if (!volunteerResult.IsSuccess)
        {
            _logger.LogWarning("Volunteer: {request.Id} not exist!", command.VolunteerId);

            return Errors.Volunteer.NotFound("volunteer").ToFailure();
        }

        var petResult = volunteerResult.Value.GetPetById(PetId.Create(command.PetId));
        if (!petResult.IsSuccess)
        {
            _logger.LogWarning("Pet {command.PetId} not exist!", command.PetId);

            return Errors.General.NotFound(command.PetId).ToFailure();
        }

        var pet = petResult.Value;

        if (pet.Photos is null || pet.Photos.Count == 0)
        {
            _logger.LogWarning("Pet: {petId} dont have these photos", command.PetId);
            return Errors.General.NotFoundValue("photos").ToFailure();
        }

        var existingPaths = pet.Photos
            .Where(photo => photo != null && photo.PathToStorage != null)
            .Select(photo => photo.PathToStorage.Path)
            .Where(path => command.Request.PhotosPaths.Contains(path))
            .ToList();

        if (existingPaths.Count == 0)
        {
            _logger.LogWarning("The requested photos were not found in the pet:{petId}", command.PetId);
            return Errors.General.NotFoundValue("photos").ToFailure();
        }

        List<PhotoMainData> photosData = [];
        foreach (var photo in existingPaths)
        {
            var photoPath = PhotoPath.Create(photo).Value;

            var photoMainData = new PhotoMainData(photoPath, BUCKET_NAME);

            photosData.Add(photoMainData);
        }

        var getResult = await _fileProvider.GetPhotos(photosData, cancellationToken);
        if (getResult.IsFailure)
            return getResult.Error.ToFailure();

        _logger.LogInformation("Get {count} photos from pet: {petId}",
            getResult.Value.Count, petResult.Value.Id.Value);

        return getResult.Value.ToList();
    }
}
