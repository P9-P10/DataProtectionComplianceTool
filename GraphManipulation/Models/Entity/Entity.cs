using System.Security.Cryptography;
using System.Text;
using GraphManipulation.Extensions;
using VDS.RDF;

namespace GraphManipulation.Models.Entity;

public abstract class Entity
{
    public readonly HashAlgorithm Algorithm = SHA1.Create();
    public string HashedFrom = null!;

    public Entity(string toHash)
    {
        ComputeId(toHash);
    }

    public string? BaseUri { get; protected set; }

    public string Id => HashToId(Hash);

    public Uri Uri
    {
        get
        {
            if (!HasBase())
            {
                throw new EntityException("Base was null when generating URI");
            }

            return UriFactory.Create(BaseUri + Id);
        }
    }

    private byte[] Hash { get; set; } = null!;

    public static string HashToId(IEnumerable<byte> hash)
    {
        var stringBuilder = new StringBuilder();

        foreach (var b in hash) stringBuilder.Append(b.ToString("x2"));

        return stringBuilder.ToString();
    }

    public abstract void UpdateBaseUri(string baseUri);

    public bool HasBase()
    {
        return BaseUri is not null;
    }

    public bool HasSameBase(string b)
    {
        return HasBase() && BaseUri.Equals(b);
    }

    public abstract string ComputeHash();

    protected void ComputeId(string toHash)
    {
        Hash = Algorithm.ComputeHash(Encoding.ASCII.GetBytes(toHash));
        HashedFrom = toHash;
    }

    public void ComputeId()
    {
        ComputeId(ComputeHash());
    }

    public override bool Equals(object? obj)
    {
        switch (obj)
        {
            case null:
                return false;
            case Entity entity:
            {
                var id1 = Id;
                var id2 = entity.Id;
                return id1 == id2;
            }
            default:
                return false;
        }
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}

public class EntityException : Exception
{
    public EntityException(string message) : base(message)
    {
    }
}