using CSharpFunctionalExtensions;
using PetFamily.Contracts.Responses;
using PetFamily.Domain.Shared;
using Shared;

namespace PetFamily.Application.FileProvider;

public interface IFileProvider
{
    Task<Result<IReadOnlyList<PhotoPath>,Error>> UploadFiles(
        IEnumerable<PhotoData> photosData,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<PhotoPath>, Error>> DeletePhotos(
        IEnumerable<PhotoMainData> photosData,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<PetPhotoUrlResponse>, Error>> GetPhotos(
        IEnumerable<PhotoMainData> photosData,
        CancellationToken cancellationToken = default);
}

