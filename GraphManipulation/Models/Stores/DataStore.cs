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

        if (HasBase() && !structure.HasSameBase(BaseUri))
        {
            structure.UpdateBaseUri(BaseUri);
        }

        structure.UpdateIdToBottom();
    }
    
    public abstract Connection GetConnection();
    
    // TODO: Lav FromDataStore metode
    // TODO: Lav ToDataStore metode?

    public override string ComputeHash()
    {
        if (BaseUri is not null)
            return BaseUri + Name;
        throw new EntityException("BaseUri was null when computing hash");
    }

    public override void UpdateBaseUri(string baseName)
    {
        BaseUri = baseName;
        ComputeId();

        foreach (var structure in SubStructures)
        {
            if (!structure.HasSameBase(baseName))
            {
                structure.UpdateBaseUri(baseName);
            }
        }
    }

    public override IGraph ToGraph()
    {
        IGraph graph = base.ToGraph();

        return graph;
    }
}