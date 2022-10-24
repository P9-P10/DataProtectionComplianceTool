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
    
    // TODO: Lav ToString

    public string? Base { get; protected set; }
    public string Id => Encoding.ASCII.GetString(Hash);
    private byte[] Hash { get; set; } = null!;

    public abstract void UpdateBase(string baseName);

    public bool HasBase()
    {
        return Base is not null;
    }
    
    public bool HasSameBase(String b)
    {
        return HasBase() && Base.Equals(b);
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

    public new IGraph ToGraph()
    {
        IGraph graph = base.ToGraph();
        
        AddUriBaseToGraph(graph);
        AddTypeToGraph(graph);

        return graph;
    }
    
    private void AddUriBaseToGraph(IGraph graph)
    {
        if (!HasBase())
        {
            throw new EntityException("Base was null when computing graph");
        }
        
        graph.BaseUri = UriFactory.Create(Base);
    }
    
    private void AddTypeToGraph(IGraph graph)
    {
        var subj = graph.CreateUriNode(UriFactory.Create(Base + Id));
        var pred = graph.CreateUriNode("rdf:type");
        var obj = graph.CreateUriNode("ddl:" + GetGraphTypeString());

        graph.Assert(subj, pred, obj);
    }
    
    protected abstract string GetGraphTypeString();
}

public class EntityException : Exception
{
    public EntityException(string message) : base(message)
    {
    }
}