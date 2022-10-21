using GraphManipulation.Models.Stores;

namespace GraphManipulation.Models.Structures;

public abstract class Structure : NamedEntity
{
    public Structure? ParentStructure;
    public DataStore? Store;
    public List<Structure> SubStructures = new();

    protected Structure(string name) : base(name)
    {
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

    public void AddToStore(DataStore store)
    {
        Store = store;
        store.AddStructure(this);
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
}

public class StructureException : Exception
{
    public StructureException(string message) : base(message)
    {
    }
}