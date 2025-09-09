using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Application.FileProvider;
using PetFamily.Application.Providers;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using PetFamily.Domain.Shared;
using Shared;
using Shared.Extensions;

namespace PetFamily.Application.Volunteers.AddPet;

public class AddPetHandler
{
    private const string BUCKET_NAME = "photos";

    private readonly IFileProvider _fileProvider;
    private readonly IVolunteersRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddPetHandler> _logger;

    public AddPetHandler(
        IFileProvider fileProvider,
        IVolunteersRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<AddPetHandler> logger)
    {
        _fileProvider = fileProvider;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid,Failure>> Handle(
        AddPetCommand command,  
        CancellationToken cancellationToken = default)
    {
        //точка старта транзакции
        var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var volunteerResult = await _repository.GetById(
               command.VolunteerId,
                cancellationToken);

            if (!volunteerResult.IsSuccess)
            {
                _logger.LogWarning("Волонтёр {request.Id} не существует!", command.VolunteerId);

                return Errors.Volunteer.NotFound("volunteer").ToFailure();
            }

            var petId = PetId.NewPetId();
            var petName = command.PetName;
            var description = command.Description;
            var healthInfo = command.HealthInfo;
            var height = command.Height;
            var weight = command.Weight;
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

            var vaccinated = command.Vaccinated;
            var petStatus = EnumExtension.ParsePetStatus(command.PetStatus).Value;
            var petColor = EnumExtension.ParsePetColor(command.Color).Value;

            //Для загрузки файлов 
            List<PhotoData> photosData = []; 
            foreach (var photo in command.Files)
            {
                var extension = Path.GetExtension(photo.FileName);

                var photoPath = PhotoPath.Create(Guid.NewGuid(), extension);
                if (photoPath.IsFailure)
                    return photoPath.Error.ToFailure();

                //Создание объекта с данными фото
                var photoContent = new PhotoData(photo.Stream, photoPath.Value, BUCKET_NAME);

                //Добавляет подготовленные данные фото в общий список.
                photosData.Add(photoContent);
            }

            //Определение параметров для хранения в БД
            //В базе данных хранятся только пути к файлам, а не сами файлы(которые могут быть большими).
            //Быстрая работа с базой данных, так как не нужно сохранять большие бинарные данные.
            //В БД не попадают бинарные данные, только ссылки на файлы в Minio.
            var petPhotos = photosData          //это для конструтора Pet
                .Select(p => p.PhotoPath)
                .Select(p => new PetPhoto(p))
                .ToList();

            var speciesId = SpeciesId.Create(command.SpeciesId);
            var breedId = BreedId.Create(command.BreedId);

            var petTypeResult = PetType.Create(speciesId, breedId);
            if(petTypeResult.IsFailure)
                return petTypeResult.Error.ToFailure();

            var pet = new Pet(
                petId,
                petName,
                description,
                healthInfo,
                address.Value,
                vaccinated,
                height,
                weight,
                petTypeResult.Value,
                DateTime.UtcNow,
                petPhotos,
                petColor,
                petStatus
                );

            volunteerResult.Value.AddPet(pet);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var uploadResult = await _fileProvider.UploadFiles(photosData, cancellationToken);

            if (uploadResult.IsFailure)
                return uploadResult.Error.ToFailure();

             transaction.Commit();

            return pet.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Can not add pet to volunteer - {id} in transaction", command.VolunteerId);

            transaction.Rollback();

            return Errors.Pet.AddToVolunteer("Can not add pet to volunteer - {id}").ToFailure();
        }
    }
}
