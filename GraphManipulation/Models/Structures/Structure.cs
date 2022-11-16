using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Stores;

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

        if (!IsTop() && !ParentStructure!.HasSameBase(baseUri))
        {
            ParentStructure.UpdateBaseUri(baseUri);
        }

        if (!IsBottom())
        {
            foreach (var subStructure in SubStructures)
            {
                if (!subStructure.HasSameBase(baseUri))
                {
                    subStructure.UpdateBaseUri(baseUri);
                }
            }
        }

        if (IsTop() && HasStore() && !Store!.HasSameBase(baseUri))
        {
            Store.UpdateBaseUri(baseUri);
        }

        ComputeId();
    }

    public void UpdateStore(DataStore store)
    {
        Store = store;
        
        if (!IsTop() && !ParentStructure!.HasSameStore(store))
        {
            ParentStructure.UpdateStore(store);
        }

        if (store.HasBase())
        {
            UpdateBaseUri(store.BaseUri);
        }

        ComputeId();

        if (IsBottom())
        {
            return;
        }

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
        if (SubStructures.Contains(structure))
        {
            return;
        }

        if (!structure.IsTop())
        {
            structure.ParentStructure!.SubStructures.Remove(structure);
        }

        SubStructures.Add(structure);
        structure.ParentStructure = this;

        if (HasStore())
        {
            UpdateStore(Store!);
        }

        if (HasBase())
        {
            UpdateBaseUri(BaseUri!);
        }

        UpdateIdToBottom();
    }

    public bool IsTop()
    {
        return ParentStructure is null;
    }

    public bool HasStore()
    {
        return Store is not null;
    }

    public bool HasSameStore(DataStore store)
    {
        return HasStore() && Store!.Equals(store);
    }

    public override List<string> ConstructIdString()
    {
        var result = new List<string>();

        if (IsTop())
        {
            if (HasStore())
            {
                result = Store!.ConstructIdString();
            }
        }
        else
        {
            result = ParentStructure!.ConstructIdString();
        }

        result.Add(Name);

        return result;
    }
}

public class StructureException : Exception
{
    public StructureException(string message) : base(message)
    {
    }
}