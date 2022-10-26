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
        IGraph graph = base.ToGraph();

        AddStructureToGraph(graph);
        
        return graph;
    }

    private void AddStructureToGraph(IGraph graph)
    {
        foreach (var subStructure in SubStructures)
        {
            graph.Merge(subStructure.ToGraph());

            var subj = graph.CreateUriNode(UriFactory.Create(BaseUri + Id));
            var pred = graph.CreateUriNode("ddl:hasStructure");
            var obj = graph.CreateUriNode(UriFactory.Create(subStructure.BaseUri + subStructure.Id));

            graph.Assert(subj, pred, obj);
        }
    }
}