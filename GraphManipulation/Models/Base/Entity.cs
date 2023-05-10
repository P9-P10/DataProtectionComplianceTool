namespace GraphManipulation.Models.Base;

public abstract class Entity<TKey> : DomainEntity where TKey : notnull
{
    public TKey Key { get; init; }

    public bool Equals(Entity<TKey> other)
    {
        return Key.Equals(other.Key);
    }
}