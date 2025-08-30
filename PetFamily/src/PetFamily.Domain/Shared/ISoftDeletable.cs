namespace PetFamily.Domain.Shared;

public interface ISoftDeletable
{
    public void Delete();
    public void Restore();
}
