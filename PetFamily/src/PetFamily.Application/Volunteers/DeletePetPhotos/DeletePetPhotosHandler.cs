using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Database;
using PetFamily.Application.FileProvider;
using PetFamily.Contracts.Commands.Volunteers;
using PetFamily.Domain.PetManagment.ValueObjects.Ids;
using PetFamily.Domain.Shared;
using Shared;

namespace PetFamily.Application.Volunteers.DeletePetPhotos
{
    public class DeletePetPhotosHandler
    {
        private const string BUCKET_NAME = "photos";

        private readonly IFileProvider _fileProvider;
        private readonly IVolunteersRepository _volunteerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<DeletePetPhotosCommand> _validator;
        private readonly ILogger<DeletePetPhotosHandler> _logger;

        public DeletePetPhotosHandler(
            IFileProvider fileProvider,
            IVolunteersRepository volunteerRepository,
            IUnitOfWork unitOfWork,
            IValidator<DeletePetPhotosCommand> validator,
            ILogger<DeletePetPhotosHandler> logger)
        {
            _fileProvider = fileProvider;
            _volunteerRepository = volunteerRepository;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<IReadOnlyList<string>, Failure>> Handle(
            DeletePetPhotosCommand command,
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
                _logger.LogWarning("Волонтёр {request.Id} не существует!", command.VolunteerId);

                return Errors.Volunteer.NotFound("volunteer").ToFailure();
            }

            var petResult = volunteerResult.Value.GetPetById(PetId.Create(command.PetId));
            if (!petResult.IsSuccess)
            {
                _logger.LogWarning("Питомец {command.PetId} не существует!", command.PetId);

                return Errors.General.NotFound(command.PetId).ToFailure();
            }

            List<PhotoMainData> photosData = [];
            foreach (var photo in command.Request.PhotoPaths)
            {
                var photoPath = PhotoPath.Create(photo).Value;

                var photoMainData = new PhotoMainData(photoPath, BUCKET_NAME);

                photosData.Add(photoMainData);
            }

            var deleteResult = await _fileProvider.DeletePhotos(photosData, cancellationToken);
            if (deleteResult.IsFailure)
                return deleteResult.Error.ToFailure();


            var deletedPaths = deleteResult.Value.Select(p => p.Path).ToList();
            petResult.Value.RemovePhotos(deletedPaths);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted {count} photos from pet: {petId}",
                deletedPaths.Count, petResult.Value.Id.Value);

            return deletedPaths;
        }
    }
}
