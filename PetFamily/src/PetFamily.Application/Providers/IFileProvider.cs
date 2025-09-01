using PetFamily.Application.FileProvider;
using Shared;

namespace PetFamily.Application.Providers;

public interface IFileProvider
{
    Task<Result<string>> UploadFile(
        FileData fileData, CancellationToken cancellationToken = default);
}
