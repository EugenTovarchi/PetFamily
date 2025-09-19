using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Application.FileProvider;
using PetFamily.Application.MessageQueue;
using PetFamily.Contracts.Commands.Volunteers;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using PetFamily.Domain.Shared;
using Shared;

namespace PetFamily.Application.Volunteers.UploadPetPhotos;

public class UploadPetPhotosHandler
{
    private const string BUCKET_NAME = "photos";

    private readonly IFileProvider _fileProvider;
    private readonly IVolunteersRepository _volunteerRepository;
    private readonly IMessageQueue<IEnumerable<PhotoMainData>> _messageQueue;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UploadPetPhotosCommand> _validator; 
    private readonly ILogger<UploadPetPhotosHandler> _logger;

    public UploadPetPhotosHandler(
        IFileProvider fileProvider,
        IVolunteersRepository volunteerRepository,
        IMessageQueue<IEnumerable<PhotoMainData>> messageQueue,
        IUnitOfWork unitOfWork,
        IValidator<UploadPetPhotosCommand> validator,
        ILogger<UploadPetPhotosHandler> logger)
    {
        _fileProvider = fileProvider;
        _messageQueue = messageQueue;
        _volunteerRepository = volunteerRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, Failure>> Handle(
        UploadPetPhotosCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if(!validationResult.IsValid)
        {
            _logger.LogError("Record {command} is invalid!", command);
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

        var petResult = volunteerResult.Value.GetPetById(PetId.Create(command.PetId));
        if (!petResult.IsSuccess)
        {
            _logger.LogWarning("Питомец {command.PetId} не существует!", command.PetId);

            return Errors.General.NotFound(command.PetId).ToFailure();
        }

        //Для загрузки файлов 
        List<PhotoData> photosData = [];
        foreach (var photo in command.Files)
        {
            var extension = Path.GetExtension(photo.FileName);

            var photoPath = PhotoPath.Create(Guid.NewGuid(), extension);
            if (photoPath.IsFailure)
                return photoPath.Error.ToFailure();

            //Создание объекта с данными фото
            var photoData = new PhotoData(photo.Stream, new PhotoMainData(photoPath.Value, BUCKET_NAME));

            //Добавляет подготовленные данные фото в общий список.
            photosData.Add(photoData);
        }

        var photosPathsResult = await _fileProvider.UploadFiles(photosData, cancellationToken);
        if (photosPathsResult.IsFailure)
        {
            await _messageQueue.WriteAsync(photosData.Select(f => f.PhotoInfo), cancellationToken); 
            return photosPathsResult.Error.ToFailure();
        }

        //Определение параметров для хранения в БД
        //В базе данных хранятся только пути к файлам, а не сами файлы(которые могут быть большими).
        //Быстрая работа с базой данных, так как не нужно сохранять большие бинарные данные.
        //В БД не попадают бинарные данные, только ссылки на файлы в Minio.
        var petPhotos = photosPathsResult.Value          
            .Select(p => new PetPhoto(p))
            .ToList();

        petResult.Value.UpdatePhotos(petPhotos);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Photos were upload to pet: {petId}", petResult.Value.Id.Value);

        return petResult.Value.Id.Value;
    }
}
