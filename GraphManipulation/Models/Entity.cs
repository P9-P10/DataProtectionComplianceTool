using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace GraphManipulation.Models;

public abstract class Entity
{
    public string? Base { get; private set; }
    public string Id => Encoding.UTF8.GetString(Hash);
    private byte[] Hash { get; set; }

    private readonly HashAlgorithm _algorithm = SHA256.Create();

    public Entity(string toHash)
    {
        ComputeId(toHash);
    }

    public void SetBase(string b)
    {
        Base = b;
    }

    public abstract string ComputeHash();

    private void ComputeId(string toHash)
    {
        Hash = _algorithm.ComputeHash(Encoding.UTF8.GetBytes(toHash)); 
    }

    public void ComputeId()
    {
        ComputeId(ComputeHash());
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;

        if (!obj.GetType().IsSubclassOf(typeof(Entity))) return false;

        // var hash1 = Hash;
        // var hash2 = (obj as Entity).Hash;
        // return hash1 == hash2;

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