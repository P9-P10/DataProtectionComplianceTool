using GraphManipulation.Models.Structures;
using VDS.RDF;

namespace GraphManipulation.Models;

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
        var subj = graph.CreateUriNode(Uri);
        var pred = graph.CreateUriNode("ddl:hasStructure");

        foreach (var subStructure in SubStructures)
        {
            graph.Merge(subStructure.ToGraph());

            var obj = graph.CreateUriNode(subStructure.Uri);

            graph.Assert(subj, pred, obj);
        }
    }
}