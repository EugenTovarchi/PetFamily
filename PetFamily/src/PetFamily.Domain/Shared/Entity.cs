namespace PetFamily.Domain.Shared;

public abstract class Entity <Tid> where Tid: notnull
{
    protected Entity (Tid id) => Id = Id;
    
    public Tid Id { get; private set; }
}
