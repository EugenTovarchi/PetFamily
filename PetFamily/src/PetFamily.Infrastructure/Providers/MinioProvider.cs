using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using PetFamily.Application.FileProvider;
using PetFamily.Contracts.Responses;
using PetFamily.Domain.Shared;
using Shared;

namespace PetFamily.Infrastructure.Providers;

public class MinioProvider : IFileProvider
{
    private const int MAX_DEGREE_OF_PARALLELISM = 10;
    private const int EXPIRY_SECONDS = 3600;

    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinioProvider> _logger;

    public MinioProvider(IMinioClient minioClient, ILogger<MinioProvider> logger)
    {
        _minioClient = minioClient;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<PhotoPath>, Error>> UploadFiles(
        IEnumerable<PhotoData> photosData,
        CancellationToken cancellationToken = default)
    {
        var semaphoreSlim = new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM);

        //копируем входящий список к List (благодаря implicit operator в ValueObjectList.cs)
        var filesList = photosData.ToList();

        try
        {
            await IfBucketsNotExistCreateBucket(photosData.Select(file => file.PhotoInfo.BucketName), cancellationToken);

            //в задачу кладем выполнение метода PutObject для каждого файла учитывая ограничение
            var tasks = filesList.Select(async file =>
                await PutObject(file, semaphoreSlim, cancellationToken));

            //Ждем пока все файлы загрузятся (максимум по 10 одновременно)
            var pathsResult = await Task.WhenAll(tasks);

            //Если есть ошибка - возвращаем первую найденную ошибку
            if (pathsResult.Any(p => p.IsFailure))
                return pathsResult.First().Error;

            //возвращаем список названий всех загруженных файлов
            var results = pathsResult.Select(p => p.Value).ToList();

            _logger.LogInformation("Success uploading files: {files}", results.Select(f => f.Path.ToList()));

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Fail to upload files in minio, files amount: {amount}", filesList.Count);

            return Error.Failure("file.upload", "Fail to upload file in minio");
        }
    }

    public async Task<Result<IReadOnlyList<PhotoPath>, Error>> DeletePhotos(
        IEnumerable<PhotoMainData> photosData,
        CancellationToken cancellationToken = default)
    {
        var filesList = photosData.ToList();
        try
        {
            var semaphoreSlim = new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM);

            var tasks = filesList.Select(async file =>
                await DeleteObjects(file, semaphoreSlim, cancellationToken));

            var pathsResult = await Task.WhenAll(tasks);

            if (pathsResult.Any(p => p.IsFailure))
                return pathsResult.First().Error;

            var results = pathsResult.Select(p => p.Value).ToList();

            _logger.LogInformation("Success deleting files: {files}", results.Select(f => f.Path.ToList()));

            return results;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fail to delete files  in Minio");

            return Error.Failure("file.delete", "Fail to delete files in minio");
        }
    }

    public async Task<Result<IReadOnlyList<PetPhotoUrlResponse>, Error>> GetPhotos(
        IEnumerable<PhotoMainData> photosData,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var results = new List<PetPhotoUrlResponse>();

            foreach (var photoData in photosData)
            {
                var bucketExistArgs = new BucketExistsArgs()
                .WithBucket(photoData.BucketName);

                var bucketExist = await _minioClient
                    .BucketExistsAsync(bucketExistArgs, cancellationToken);

                if (bucketExist == false)
                    return Errors.General.NotFoundValue("bucket");

                var args = new PresignedGetObjectArgs()
                    .WithBucket(photoData.BucketName)
                    .WithObject(photoData.PhotoPath.ToString())
                    .WithExpiry(EXPIRY_SECONDS);

                string presignedUrl = await _minioClient.PresignedGetObjectAsync(args);

                results.Add(new PetPhotoUrlResponse(
                    PhotoPath: photoData.PhotoPath,
                    Url: presignedUrl,
                    ExpiresAt: DateTime.UtcNow.AddSeconds(EXPIRY_SECONDS)
                ));
            }

            if (results.Count == 0)
                return Errors.General.NotFoundValue("photos");

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fail to get files in Minio");

            return Error.Failure("Minio.GetUrl.Failed", "Failed to generate URLs");

        }
    }

    private async Task<Result<PhotoPath, Error>> DeleteObjects(
        PhotoMainData photoData,
        SemaphoreSlim semaphoreSlim,
        CancellationToken cancellationToken)
    {
        await semaphoreSlim.WaitAsync(cancellationToken);

        var bucketExistArgs = new BucketExistsArgs()
                .WithBucket(photoData.BucketName);

        var bucketExist = await _minioClient
            .BucketExistsAsync(bucketExistArgs, cancellationToken);

        if (bucketExist == false)
            return Errors.General.NotFoundValue("bucket");


        var args = new RemoveObjectsArgs()
        .WithBucket(photoData.BucketName)
        .WithObject(photoData.PhotoPath.Path);

        try
        {
            await _minioClient.RemoveObjectsAsync(args, cancellationToken);

            return photoData.PhotoPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Fail to delete file in minio with path {path} from bucket {bucket}",
                photoData.PhotoPath.Path,
                photoData.BucketName);

            return Error.Failure("file.delete", "Fail to delete file in minio");
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    //Удаление по 1му файлу из minio в BackgroundService
    public async Task<UnitResult<Error>> RemovePhoto(
        PhotoMainData photoInfo,
        CancellationToken ct)
    {
        try
        {
            var statArgs = new StatObjectArgs()
                .WithBucket(photoInfo.BucketName)
                .WithObject(photoInfo.PhotoPath.Path);

            //Проверка на то, что файлы существуют в Minio по собранным аргументам
            var statObject = await _minioClient.StatObjectAsync(statArgs, ct); //тут нужны права в MInio 
            if (statObject == null)
                return UnitResult.Success<Error>();

            await IfBucketsNotExistCreateBucket([photoInfo.BucketName], ct);

            var removeArgs = new RemoveObjectArgs()
              .WithBucket(photoInfo.BucketName)
              .WithObject(photoInfo.PhotoPath.Path);

            await _minioClient.RemoveObjectAsync(removeArgs, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
               "Fail to remove file in minio with path {path} in bucket {bucket}",
               photoInfo.PhotoPath.Path,
               photoInfo.BucketName);

            return Error.Failure("file.remove", "Fail to remove file in minio");
        }

        return UnitResult.Success<Error>();
    }

    private async Task<Result<PhotoPath, Error>> PutObject(
        PhotoData photoData,
        SemaphoreSlim semaphoreSlim,
        CancellationToken cancellationToken)
    {
        //Каждая задача "стоит в очереди" перед дверью - устанавливаем ограничение для потоков/задач
        await semaphoreSlim.WaitAsync(cancellationToken);

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(photoData.PhotoInfo.BucketName)
            .WithStreamData(photoData.Stream)
            .WithObjectSize(photoData.Stream.Length)
            .WithObject(photoData.PhotoInfo.PhotoPath.Path);

        try
        {
            //Вызываем метод загрузки у MinioClient предварительно собрав для этого параметры 
            await _minioClient
                .PutObjectAsync(putObjectArgs, cancellationToken);

            return photoData.PhotoInfo.PhotoPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Fail to upload file in minio with path {path} in bucket {bucket}",
                photoData.PhotoInfo.PhotoPath.Path,
                photoData.PhotoInfo.BucketName);

            return Error.Failure("file.upload", "Fail to upload file in minio");
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private async Task IfBucketsNotExistCreateBucket(
        IEnumerable<string> buckets,
        CancellationToken cancellationToken)
    {
        //преобразуем в коллекцию уникальных папок - убираем дубликаты
        HashSet<string> bucketNames = [.. buckets]; //получаем bucketName у каждого файла(внутри рекорда есть свойство)

        foreach (var bucketName in bucketNames)
        {
            //собираем параметры(BucketExistsArgs) для запроса BucketExistsAsync
            var bucketExistArgs = new BucketExistsArgs()
                .WithBucket(bucketName);

            //проверяем существуют ли такие buckets в хранилище ?
            var bucketExist = await _minioClient
                .BucketExistsAsync(bucketExistArgs, cancellationToken);

            if (bucketExist == false)
            {
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(bucketName);

                //создаем папку(bucket) в случае, если её не было
                await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
            }
        }
    }
}
