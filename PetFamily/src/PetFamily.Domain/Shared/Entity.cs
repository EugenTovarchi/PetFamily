namespace PetFamily.Domain.Shared;

public abstract class Entity <TId> where TId: notnull
{
    protected Entity (TId id) => Id = id;
    
    public TId Id { get; private set; }

    /// <summary>
    /// Обеспечивает логическое равенство сущностей
    /// (две сущности равны, если их Id одинаковы, даже если это разные объекты в памяти).
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType()) 
        return false;

        var other = (Entity<TId>) obj;
        return ReferenceEquals(this, other) || Id.Equals(other.Id);  //Сравнение по ссылке и Id
    }

    /// <summary>
    /// Гарантирует, что две сущности с одинаковым Id дают одинаковый хеш
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return (GetType().FullName + Id).GetHashCode();
    }

    /// <summary>
    /// Сравнение по ссылке
    /// Позволяет сравнивать сущности через == (удобный синтаксис вместо .Equals())
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if(ReferenceEquals(left, null)  && ReferenceEquals(right, null)) return true;

        if(ReferenceEquals(left, null) || ReferenceEquals(right, null)) { return false; }

        return left.Equals(right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !(left==right);
    }
}
