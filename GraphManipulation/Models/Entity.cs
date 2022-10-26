using System.Security.Cryptography;
using System.Text;
using VDS.RDF;

namespace GraphManipulation.Models;

public abstract class Entity : GraphBased
{
    public readonly HashAlgorithm Algorithm = SHA256.Create();
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
            if (!HasBase()) throw new EntityException("Base was null when generating URI");

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

    public abstract void UpdateBaseUri(string baseName);

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

    public override IGraph ToGraph()
    {
        var graph = base.ToGraph();

        AddUriBaseToGraph(graph);
        AddTypeToGraph(graph);

        return graph;
    }

    private void AddUriBaseToGraph(IGraph graph)
    {
        if (!HasBase()) throw new EntityException("BaseUri was null when building graph");

        graph.BaseUri = UriFactory.Create(BaseUri);
    }

    private void AddTypeToGraph(IGraph graph)
    {
        var triple = new Triple(
            graph.CreateUriNode(Uri),
            graph.CreateUriNode("rdf:type"),
            graph.CreateUriNode("ddl:" + GetGraphTypeString())
        );

        graph.Assert(triple);
    }

    protected abstract string GetGraphTypeString();
}

public class EntityException : Exception
{
    public EntityException(string message) : base(message)
    {
    }
}