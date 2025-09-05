using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using PetFamily.Application.FileProvider;
using PetFamily.Application.Providers;
using PetFamily.Domain.Shared;
using Shared;

namespace PetFamily.Infrastructure.Providers;

public class MinioProvider : IFileProvider
{
    private const int MAX_DEGREE_OF_PARALLELISM = 10;

    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinioProvider> _logger;

    public MinioProvider(IMinioClient minioClient, ILogger<MinioProvider> logger)
    {
        _minioClient = minioClient;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<PhotoPath>>> UploadFiles(
        IEnumerable<PhotoData> photosData,
        CancellationToken cancellationToken = default)
    {
        var semaphoreSlim = new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM);

        //копируем входящий список к List (благодаря implicit operator в ValueObjectList.cs)
        var filesList = photosData.ToList(); 

        try
        {
            await IfBucketsNotExistCreateBucket(filesList, cancellationToken);

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

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Fail to upload files in minio, files amount: {amount}", filesList.Count);

            return Error.Failure("file.upload", "Fail to upload files in minio");
        }
    }

    private async Task<Result<PhotoPath>> PutObject(
        PhotoData photoData,
        SemaphoreSlim semaphoreSlim,
        CancellationToken cancellationToken)
    {
        //Каждая задача "стоит в очереди" перед дверью - устанавливаем ограничение для потоков/задач
        await semaphoreSlim.WaitAsync(cancellationToken);

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(photoData.BucketName)
            .WithStreamData(photoData.Stream)
            .WithObjectSize(photoData.Stream.Length) //берем размер файла из Stream
            .WithObject(photoData.PhotoPath.Path);  //ВОПРОС - PhotoPath чем является? названием файла ?

        try
        {
            //Вызываем метод загрузки у MinioClient предварительно собрав для этого параметры 
            await _minioClient
                .PutObjectAsync(putObjectArgs, cancellationToken);

            return photoData.PhotoPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Fail to upload file in minio with path {path} in bucket {bucket}",
                photoData.PhotoPath.Path,
                photoData.BucketName);

            return Error.Failure("file.upload", "Fail to upload file in minio");
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private async Task IfBucketsNotExistCreateBucket(
        IEnumerable<PhotoData> photosData,
        CancellationToken cancellationToken)
    {
        //преобразуем в коллекцию уникальных папок - убираем дубликаты
        HashSet<string> bucketNames = [.. photosData.Select(file => file.BucketName)]; //получаем bucketName у каждого файла(внутри рекорда есть свойство)

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
