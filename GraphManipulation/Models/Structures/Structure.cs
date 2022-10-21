using GraphManipulation.Interfaces;
using GraphManipulation.Models.Stores;
using VDS.RDF;

namespace GraphManipulation.Models.Structures;

public abstract class Structure : NamedEntity, IHasStructure
{
    public Structure? ParentStructure;
    public DataStore? Store;
    public List<Structure> SubStructures = new();

    protected Structure(string name) : base(name)
    {
    }

    protected override void UpdateBase(string b)
    {
        throw new NotImplementedException();
    }

    protected void UpdateStore(DataStore store)
    {
        Store = store;

        if (!IsTop() && !ParentStructure.HasSameStore(store))
        {
            ParentStructure.UpdateStore(store);
        }
        if (!IsBottom())
        {
            foreach (var subStructure in SubStructures)
            {
                if (!subStructure.HasSameStore(store))
                {
                    subStructure.UpdateStore(store);
                }
            }
        }
    }

    public bool HasSameStore(DataStore store)
    {
        return HasStore() && Store.Equals(store);
    }

    public void AddStructure(Structure structure)
    {
        if (SubStructures.Contains(structure)) return;

        SubStructures.Add(structure);
        structure.ParentStructure = this;
        UpdateToBottom();
    }

    public void UpdateToBottom()
    {
        ComputeId();

        if (!IsBottom())
            foreach (var subStructure in SubStructures)
                subStructure.UpdateToBottom();
    }

    public void SetStore(DataStore store)
    {
        UpdateStore(store);
    }

    public bool IsTop()
    {
        return ParentStructure is null;
    }

    public bool IsBottom()
    {
        return SubStructures.Count == 0;
    }

    public bool HasStore()
    {
        return Store is not null;
    }

    public override string ComputeHash()
    {
        var result = "";

        if (IsTop())
        { 
            if (HasStore()) result += Store.ComputeHash();
        }
        else
        {
            result += ParentStructure.ComputeHash();
        }

        result += Name;

        return result;
    }

    public override IGraph ToGraph()
    {
        IGraph graph = new Graph();
        AddNamespaces(graph);
        
        if (!HasBase())
        {
            throw new EntityException("Base was null when computing graph");
        }
        
        graph.BaseUri = UriFactory.Create(Base);
        
        string className = "ddl:";

        switch (this)
        {
            case Column:
                className += "Column";
                break;
            case Schema:
                className += "Schema";
                break;
            case Table:
                className += "Table";
                break;
            default:
                throw new GraphBasedException("Structure type was is not supported");
        }

        var subj = graph.CreateUriNode(UriFactory.Create(Base + Id));
        var pred = graph.CreateUriNode("rdf:type");
        var obj = graph.CreateUriNode(className);


        graph.Assert(new Triple(subj, pred, obj));

        return graph;
    }

    public override IGraphBased FromGraph(IGraph graph)
    {
        throw new NotImplementedException();
    }
}

public class StructureException : Exception
{
    public StructureException(string message) : base(message)
    {
    }
}