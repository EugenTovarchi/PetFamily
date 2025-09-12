using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using Shared;

namespace PetFamily.Application.FileProvider;

public interface IFileProvider
{
    Task<Result<IReadOnlyList<PhotoPath>,Error>> UploadFiles(
        IEnumerable<PhotoData> photosData,
        CancellationToken cancellationToken = default);
}

