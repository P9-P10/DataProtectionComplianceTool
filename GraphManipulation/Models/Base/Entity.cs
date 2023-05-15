using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Models.Base;

public abstract class Entity<TKey> : DomainEntity, IListable
{
    public TKey? Key { get; set; }
    public string? Description { get; set; }

    public void UpdateUsing(object? other)
    {
        if (other is null)
        {
            return;
        }

        var properties = other.GetType().GetProperties().Where(p => p.CanWrite);

        foreach (var property in properties)
        {
            var value = property.GetValue(other, null);

            if (value is not null)
            {
                property.SetValue(this, value, null);
            }
        }
    }

    public override bool Equals(object? obj)
    {
        return obj is not null && Equals((obj as Entity<TKey>)!);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<TKey?>.Default.GetHashCode(Key);
    }

    public bool Equals(Entity<TKey> other)
    {
        return other.ToListing().Equals(ToListing());
    }

    public override string ToString()
    {
        return ToListing();
    }

    public virtual string ToListing()
    {
        return string.Join(", ", Key?.ToString() ?? "No key found", Description);
    }

    public virtual string ToListingIdentifier()
    {
        return Key?.ToString() ?? "No key found";
    }
}