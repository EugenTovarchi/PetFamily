namespace PetFamily.Application.FileProvider;

public interface IFilesCleanerService
{
    Task Process(CancellationToken ct);
}
