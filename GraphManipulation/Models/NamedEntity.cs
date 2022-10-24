using VDS.RDF;

namespace GraphManipulation.Models;

public abstract class NamedEntity : Entity
{
    public NamedEntity(string name) : base(name)
    {
        Name = name;
    }

    public string Name { get; }

    public new IGraph ToGraph()
    {
        IGraph graph = base.ToGraph();
        
        AddNameToGraph(graph);

        return graph;
    }

    private void AddNameToGraph(IGraph graph)
    {
        var subj = graph.CreateUriNode(UriFactory.Create(Base + Id));
        var pred = graph.CreateUriNode("ddl:hasName");
        var obj = graph.CreateLiteralNode(Name);

        graph.Assert(subj, pred, obj);
    }
}