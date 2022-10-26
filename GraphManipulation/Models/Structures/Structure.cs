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

    public override void UpdateBaseUri(string baseName)
    {
        BaseUri = baseName;

        if (!IsTop() && !ParentStructure.HasSameBase(baseName))
        {
            ParentStructure.UpdateBaseUri(baseName);
        }
        if (!IsBottom())
        {
            foreach (var subStructure in SubStructures)
            {
                if (!subStructure.HasSameBase(baseName))
                {
                    subStructure.UpdateBaseUri(baseName);
                }
            }
        }

        if (IsTop() && HasStore() && !Store.HasSameBase(baseName))
        {
            Store.UpdateBaseUri(baseName);
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

        if (IsBottom()) return;
        
        foreach (var subStructure in SubStructures)
        {
            if (!subStructure.HasSameStore(store))
            {
                subStructure.UpdateStore(store);
            }
        }
    }

    public override void AddStructure(Structure structure)
    {
        if (SubStructures.Contains(structure)) return;

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

    public IGraph AddHasStructureToGraph(Structure structure)
    {
        throw new NotImplementedException();
    }

    public void UpdateIdToBottom()
    {
        ComputeId();

        if (!IsBottom())
            foreach (var subStructure in SubStructures)
                subStructure.UpdateIdToBottom();
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
        IGraph baseGraph = base.ToGraph();
    
        return baseGraph;
    }
}

public class StructureException : Exception
{
    public StructureException(string message) : base(message)
    {
    }
}