using System.Collections;
using System.Security.Cryptography;
using GraphManipulation.Models.Stores;

namespace GraphManipulation.Models.Structures;

public abstract class Structure : NamedEntity
{
    public readonly Hashtable SubStructures = new Hashtable();
    private Structure? _predecessor;
    private DataStore? _store;  

    protected Structure(string name) : base(name)
    {
    }

    public void AddStructure(Structure structure)
    {
        SubStructures.Add(structure, structure);
        structure._predecessor = this;
    }

    public void SetStore(DataStore store)
    {
        _store = store;
    }

    private bool IsTop()
    {
        return _predecessor is null;
    }

    public override string ComputeHash()
    {
        string result = "";

        if (IsTop())
        {
            if (_store is not null)
            {
                result += _store.ComputeHash();
            }
            else
            {
                throw new StructureException("Structure at top does not have Store");
            }
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