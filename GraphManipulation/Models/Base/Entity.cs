using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Models.Base;

public class Entity<TKey> : DomainEntity, IListable
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
        return string.Join(", ", ToListingIdentifier(), Description ?? "None");
    }

    public virtual string ToListingIdentifier()
    {
        return Key?.ToString() ?? "No key";
    }

    protected string NullOrEmptyToString<T>(T? obj, Func<T, bool> isEmpty, Func<T, string> toString)
    {
        return obj is null || isEmpty(obj) ? "Empty" : toString(obj);
    }

    protected string ListNullOrEmptyToString<T>(IEnumerable<T>? list, Func<IEnumerable<T>, string> toString)
    {
        return NullOrEmptyToString(list, enumerable => !enumerable.Any(), toString);
    }

    protected string ListNullOrEmptyToString<T>(IEnumerable<Entity<T>>? list)
    {
        return ListNullOrEmptyToString(list, EncapsulateList);
    }
    
    protected string NullToString<T>(T? obj, Func<T, string> toString)
    {
        return obj is null ? "None" : toString(obj);
    }

    protected string NullToString(object? obj)
    {
        return NullToString(obj, o => o.ToString());
    }

    protected string NullToString(string? obj)
    {
        return NullToString(obj, s => s);
    }

    protected string NullToString<T>(Entity<T>? entity)
    {
        return NullToString(entity, e => e.ToListingIdentifier());
    }

    protected string EncapsulateList(IEnumerable<string> list)
    {
        return "[ " + string.Join(", ", list) + " ]";
    }

    protected string EncapsulateList<T>(IEnumerable<Entity<T>> list)
    {
        return "[ " + string.Join(", ", list.Select(e => e.ToListingIdentifier())) + " ]";
    }
}