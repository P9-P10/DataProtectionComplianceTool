using GraphManipulation.Interfaces;
using GraphManipulation.Models.Connections;
using GraphManipulation.Models.Structures;
using VDS.RDF;

namespace GraphManipulation.Models.Stores;

public abstract class DataStore : NamedEntity, IHasStructure
{
    public List<Structure> Structures = new();

    protected DataStore(string name) : base(name)
    {
    }

    public void AddStructure(Structure structure)
    {
        if (Structures.Contains(structure)) return;

        Structures.Add(structure);

        if (!structure.HasStore() || !structure.Store.Equals(this)) structure.SetStore(this);

        structure.UpdateToBottom();
    }

    public abstract Connection GetConnection();
    
    // TODO: Lav FromDataStore metode
    // TODO: Lav ToDataStore metode?

    public override string ComputeHash()
    {
        if (Base is not null)
            return Base + Name;
        throw new EntityException("Base was null when computing hash");
    }

    protected override void UpdateBase(string b)
    {
        throw new NotImplementedException();
    }
}