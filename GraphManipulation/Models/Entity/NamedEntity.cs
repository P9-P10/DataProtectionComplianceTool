using VDS.RDF;

namespace GraphManipulation.Models.Entity;

public abstract class NamedEntity : Entity
{
    public NamedEntity(string name) : base(name)
    {
        Name = name;
    }

    public string Name { get; protected set; }

    public override IGraph ToGraph()
    {
        var graph = base.ToGraph();

        AddNameToGraph(graph);

        return graph;
    }

    private void AddNameToGraph(IGraph graph)
    {
        var triple = new Triple(
            graph.CreateUriNode(Uri),
            graph.CreateUriNode("ddl:hasName"),
            graph.CreateLiteralNode(Name)
        );

        graph.Assert(triple);
    }
}