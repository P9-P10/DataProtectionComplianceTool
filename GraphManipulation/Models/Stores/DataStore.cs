using GraphManipulation.Models.Connections;
using GraphManipulation.Models.Structures;
using VDS.RDF;

namespace GraphManipulation.Models.Stores;

public abstract class DataStore : StructuredEntity
{
    protected DataStore(string name) : base(name)
    {
    }

    public override void AddStructure(Structure structure)
    {
        if (SubStructures.Contains(structure)) return;

        SubStructures.Add(structure);

        if (!structure.HasSameStore(this))
        {
            structure.UpdateStore(this);
        }

        if (HasBase() && !structure.HasSameBase(Base))
        {
            structure.UpdateBase(Base);
        }

        structure.UpdateIdToBottom();
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

    public override void UpdateBase(string baseName)
    {
        Base = baseName;
        ComputeId();

        foreach (var structure in SubStructures)
        {
            if (!structure.HasSameBase(baseName))
            {
                structure.UpdateBase(baseName);
            }
        }
    }

    public override IGraph ToGraph()
    {
        IGraph graph = base.ToGraph();

        return graph;
    }
}