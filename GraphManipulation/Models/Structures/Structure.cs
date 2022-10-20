using GraphManipulation.Models.Stores;

namespace GraphManipulation.Models.Structures;

public abstract class Structure : NamedEntity
{
    private Structure? _predecessor;
    private DataStore? _store;
    public List<Structure> SubStructures = new();

    protected Structure(string name) : base(name)
    {
    }

    public void AddStructure(Structure structure)
    {
        SubStructures.Add(structure);
        structure._predecessor = this;
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
        _store = store;
    }

    private bool IsTop()
    {
        return _predecessor is null;
    }

    private bool IsBottom()
    {
        return SubStructures.Count == 0;
    }

    public override string ComputeHash()
    {
        var result = "";

        if (IsTop())
        {
            if (_store is not null)
                result += _store.ComputeHash();
            else
                throw new StructureException("Structure at top does not have Store");
        }
        else
        {
            result += _predecessor.ComputeHash();
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