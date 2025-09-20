using Microsoft.Extensions.Logging;
using PetFamily.Application.FileProvider;
using PetFamily.Application.MessageQueue;

namespace PetFamily.Infrastructure.FileServices;

public class PhotosCleanerService : IFilesCleanerService
{
    private readonly IMessageQueue<IEnumerable<PhotoMainData>> _messageQueue;
    private readonly ILogger<PhotosCleanerService> _logger;
    private readonly IFileProvider _fileProvider;

    public PhotosCleanerService(
        IMessageQueue<IEnumerable<PhotoMainData>> messageQueue,
        ILogger<PhotosCleanerService> logger,
        IFileProvider fileProvider)
    {
        _messageQueue = messageQueue;
        _logger = logger;
        _fileProvider = fileProvider;
    }

    public async Task Process(CancellationToken ct)
    {
        var photoInfos = await _messageQueue.ReadAsync(ct);

        foreach (var photoInfo in photoInfos)
        {
            _logger.LogDebug(
                       "Try to remove photo: {PhotoPath} from bucket: {BucketName}",
                       photoInfo.PhotoPath.Path,
                       photoInfo.BucketName);

            var result = await _fileProvider.RemovePhoto(photoInfo, ct);

            if(result.IsSuccess)
            {
                _logger.LogInformation(
                           "Successfully removed photo: {PhotoPath} from bucket: {BucketName}",
                           photoInfo.PhotoPath.Path,
                           photoInfo.BucketName);
            }
            else
            {
                _logger.LogWarning(
                            "Failed to remove photo: {PhotoPath} from bucket: {BucketName}. Error: {ErrorCode} - {ErrorMessage}",
                            photoInfo.PhotoPath.Path,
                            photoInfo.BucketName,
                            result.Error.Code,
                            result.Error.Message);
            }
        }
    }
}
