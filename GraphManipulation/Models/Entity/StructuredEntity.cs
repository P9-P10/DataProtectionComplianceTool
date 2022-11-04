using GraphManipulation.Extensions;
using GraphManipulation.Models.Structures;
using VDS.RDF;

namespace GraphManipulation.Models.Entity;

public abstract class StructuredEntity : NamedEntity
{
    public List<Structure> SubStructures = new();

    protected StructuredEntity(string name) : base(name)
    {
    }

    public abstract void AddStructure(Structure structure);

    public override IGraph ToGraph()
    {
        var graph = base.ToGraph();

        AddStructureToGraph(graph);

        return graph;
    }

    private void AddStructureToGraph(IGraph graph)
    {
        foreach (var subStructure in SubStructures)
        {
            graph.Merge(subStructure.ToGraph());
            
            graph.AssertHasStructureTriple(this, subStructure);
        }
    }
}