using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Stores;
using VDS.RDF;

namespace GraphManipulation.Models.Structures;

public abstract class Structure : StructuredEntity //, IHasStructure
{
    public Structure? ParentStructure;
    public DataStore? Store;

    protected Structure(string name) : base(name)
    {
    }

    public override void UpdateBaseUri(string baseUri)
    {
        BaseUri = baseUri;

        if (!IsTop() && !ParentStructure.HasSameBase(baseUri))
        {
            ParentStructure.UpdateBaseUri(baseUri);
        }

        if (!IsBottom())
        {
            foreach (var subStructure in SubStructures)
                if (!subStructure.HasSameBase(baseUri))
                {
                    subStructure.UpdateBaseUri(baseUri);
                }
        }

        if (IsTop() && HasStore() && !Store.HasSameBase(baseUri))
        {
            Store.UpdateBaseUri(baseUri);
        }

        ComputeId();
    }

    public void UpdateStore(DataStore store)
    {
        Store = store;

        if (!IsTop() && !ParentStructure.HasSameStore(store))
        {
            ParentStructure.UpdateStore(store);
        }

        ComputeId();

        if (IsBottom())
        {
            return;
        }

        foreach (var subStructure in SubStructures)
            if (!subStructure.HasSameStore(store))
            {
                subStructure.UpdateStore(store);
            }
    }

    public override void AddStructure(Structure structure)
    {
        if (SubStructures.Contains(structure))
        {
            return;
        }

        if (!structure.IsTop())
        {
            structure.ParentStructure.SubStructures.Remove(structure);
        }

        SubStructures.Add(structure);
        structure.ParentStructure = this;

        if (HasStore())
        {
            UpdateStore(Store);
        }

        if (HasBase())
        {
            UpdateBaseUri(BaseUri);
        }

        UpdateIdToBottom();
    }

    public void UpdateIdToBottom()
    {
        ComputeId();

        if (!IsBottom())
        {
            foreach (var subStructure in SubStructures)
                subStructure.UpdateIdToBottom();
        }
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

    public bool HasSameStore(DataStore store)
    {
        return HasStore() && Store.Equals(store);
    }

    public override string ComputeHash()
    {
        var result = "";

        if (IsTop())
        {
            if (HasStore())
            {
                result += Store.ComputeHash();
            }
            else if (HasBase())
            {
                result += BaseUri;
            }
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
        var baseGraph = base.ToGraph();

        AddHasStoreToGraph(baseGraph);

        return baseGraph;
    }

    private void AddHasStoreToGraph(IGraph graph)
    {
        if (!HasStore())
        {
            throw new StructureException("Store was null when building graph");
        }

        var triple = new Triple(
            graph.CreateUriNode(Uri),
            graph.CreateUriNode("ddl:hasStore"),
            graph.CreateUriNode(Store.Uri));

        graph.Assert(triple);
    }
}

public class StructureException : Exception
{
    public StructureException(string message) : base(message)
    {
    }
}