using PetFamily.Application.FileProvider;
using PetFamily.Domain.Shared;
using Shared;

namespace PetFamily.Application.Providers;

public interface IFileProvider
{
    Task<Result<IReadOnlyList<PhotoPath>>> UploadFiles(
        IEnumerable<PhotoData> photosData,
        CancellationToken cancellationToken = default);
}

