using System.Security.Cryptography;
using System.Text;
using VDS.RDF;

namespace GraphManipulation.Models.Entity;

public abstract class Entity : IEquatable<Entity>
{
    public static readonly char IdSeparator = '/'; 
    
    protected Entity(string id)
    {
        Id = id;
    }

    private string _baseUri = "";

    public string? BaseUri
    {
        get => string.IsNullOrEmpty(_baseUri) ? null : _baseUri;
        protected set
        {
            if (value is null)
            {
                _baseUri = "";
                return;
            }
            
            if (Uri.TryCreate(value, UriKind.Absolute, out _))
            {
                _baseUri = value;
            }
            else
            {
                throw new EntityException("Uri given as BaseUri not valid Uri: " + value);
            }
        }
    }

    public string Id;

    public Uri Uri
    {
        get
        {
            if (!HasBase())
            {
                throw new EntityException("Base was null when generating URI");
            }

            if (Uri.TryCreate(Id, UriKind.Absolute, out var uri))
            {
                return uri;
            }

            throw new EntityException("Something went wrong when creating uri from: " + BaseUri + Id);
        }
    }

    public abstract void UpdateBaseUri(string baseUri);

    public bool HasBase()
    {
        return BaseUri is not null;
    }

    public bool HasSameBase(string b)
    {
        return HasBase() && BaseUri!.Equals(b);
    }

    public abstract string ConstructIdString();

    public void ComputeId()
    {
        Id = ConstructIdString().TrimEnd(IdSeparator);
    }
    
    public bool Equals(Entity? other)
    {
        return other is not null && Id == other.Id;
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
        return string.GetHashCode(Id);
    }
}

public class EntityException : Exception
{
    public EntityException(string message) : base(message)
    {
    }
}