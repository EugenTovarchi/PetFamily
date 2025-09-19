namespace PetFamily.Application.FileProvider;

/// <summary>
/// Запись для загрузки фото в Minio
/// </summary>
/// <param name="Stream"></param>
/// <param name="PhotoPath"></param>
/// <param name="BucketName"></param>
public record PhotoData(Stream Stream, PhotoMainData PhotoInfo);
