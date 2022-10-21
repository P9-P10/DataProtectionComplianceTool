using System.Security.Cryptography;
using System.Text;

namespace GraphManipulation.Models;

public abstract class Entity
{
    public readonly HashAlgorithm Algorithm = SHA256.Create();
    public string HashedFrom = null!;

    public Entity(string toHash)
    {
        ComputeId(toHash);
    }

    public string? Base { get; private set; }
    public string Id => Encoding.UTF8.GetString(Hash);
    private byte[] Hash { get; set; } = null!;

    public void SetBase(string b)
    {
        Base = b;
        ComputeId();
    }

    public abstract string ComputeHash();

    protected void ComputeId(string toHash)
    {
        Hash = Algorithm.ComputeHash(Encoding.UTF8.GetBytes(toHash));
        HashedFrom = toHash;
    }

    public void ComputeId()
    {
        ComputeId(ComputeHash());
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        if (!obj.GetType().IsSubclassOf(typeof(Entity))) return false;

        var id1 = Id;
        var id2 = (obj as Entity).Id;
        return id1 == id2;
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