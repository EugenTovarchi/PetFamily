namespace PetFamily.Domain.Shared;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    public DateTime? DeletionDate { get;  }
    public void Delete();
    public void Restore();
}
