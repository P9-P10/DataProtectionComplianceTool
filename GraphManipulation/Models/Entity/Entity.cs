namespace GraphManipulation.Models.Entity;

public abstract class Entity : IEquatable<Entity>
{
    public const char IdSeparator = '/';

    private string _baseUri = "";

    private List<string> _id;

    protected Entity(string id)
    {
        _id = new List<string> { id };
    }

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

            if (value.Last() is not (IdSeparator or '#'))
            {
                throw new EntityException($"Base uri must end on {IdSeparator} or '#'");
            }

            if (IsValidUri(value))
            {
                _baseUri = value;
            }
            else
            {
                throw new EntityException("Uri given as BaseUri not valid Uri: " + value);
            }
        }
    }

    public string Id => string.Join(IdSeparator, _id);

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

            throw new EntityException("Something went wrong when creating uri from: " + Id);
        }
    }


    public bool Equals(Entity? other)
    {
        return other is not null && Id == other.Id;
    }

    public static bool IsValidUri(string maybeUri)
    {
        return Uri.TryCreate(maybeUri, UriKind.RelativeOrAbsolute, out _);
    }

    public void ComputeId()
    {
        _id = BaseUri is null
            ? ConstructIdString()
            : ConstructIdString()
                .Prepend(BaseUri.TrimEnd(IdSeparator, '#'))
                .ToList();
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

    public abstract List<string> ConstructIdString();

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