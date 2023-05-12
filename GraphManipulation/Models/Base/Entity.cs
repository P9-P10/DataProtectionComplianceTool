using System.Reflection;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Models.Base;

public abstract class Entity<TKey> : DomainEntity, IListable
{
    public TKey? Key { get; set; }
    public string? Description { get; set; }

    public void Fill(object? other)
    {
        if (other is null)
        {
            return;
        }

        foreach (var property in other.GetType().GetProperties().Where(p => p.CanWrite))
        {
            property.SetValue(other, property.GetValue(this, null), null);
        }
        
        // if (other is null || (other.GetType() != typeof(Entity<TKey>) && !other.GetType().IsSubclassOf(typeof(Entity<TKey>))))
        // {
        //     return;
        // }
        //
        // var otherEntity = (other as Entity<TKey>)!;
        //
        // otherEntity.Id ??= Id;
        // otherEntity.Key ??= Key;
        // otherEntity.Description ??= Description;
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

    public string ToListingIdentifier()
    {
        return Key?.ToString() ?? "No key found";
    }
}