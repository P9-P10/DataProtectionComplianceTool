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

        if (!structure.HasSameStore(this))
        {
            structure.UpdateStore(this);
        }

        if (!structure.HasSameBase(Base))
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

        foreach (var structure in Structures)
        {
            if (!structure.HasSameBase(baseName))
            {
                structure.UpdateBase(baseName);
            }
        }
    }

    public new IGraph ToGraph()
    {
        IGraph graph = base.ToGraph();

        foreach (var structure in Structures)
        {
            graph.Merge(structure.ToGraph());
        }
        
        return graph;
    }
    
    // private void AddHasStructureToGraph(IGraph graph)
    // {
    //     var subj = graph.CreateUriNode(UriFactory.Create(Base + Id));
    //     var pred = graph.CreateUriNode("ddl:hasStructure");
    //     
    //     var obj = graph.CreateUriNode(UriFactory.Create(structure.Base + structure.Id));
    //
    //     graph.Assert(subj, pred, obj);
    // }
}