using System.Collections;
using System.Security.Cryptography;
using GraphManipulation.Interfaces;
using GraphManipulation.Models.Connections;
using GraphManipulation.Models.Structures;

namespace GraphManipulation.Models.Stores;

public abstract class DataStore : NamedEntity, IHasStructure
{
    public Hashtable DataStoreStructures = new Hashtable();
    public abstract Connection GetConnection();
    public abstract string ToRdf();
    public abstract void FromRdf();

    protected DataStore(string name) : base(name)
    {
    }

    public void AddStructure(Structure structure)
    {
        DataStoreStructures.Add(structure, structure);
        structure.SetStore(this);
    }
    
    public override string ComputeHash()
    {
        if (Base is not null)
        {
            return Base + Name;
        }
        else
        {
            throw new EntityException("Base was null when computing hash");
        }
    }
}