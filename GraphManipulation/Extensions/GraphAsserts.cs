using GraphManipulation.Models.Entity;
using VDS.RDF;

namespace GraphManipulation.Extensions;

public static class GraphAsserts
{
    public static void AssertHasStructureTriple(this IGraph graph, Entity parent, Entity child)
    {
        var subj = graph.CreateUriNode(parent.Uri);
        var pred = graph.CreateUriNode("ddl:hasStructure");
        var obj = graph.CreateUriNode(child.Uri);

        graph.Assert(subj, pred, obj);
    }

    public static void AssertTypeTriple(this IGraph graph, Entity entity)
    {
        var subj = graph.CreateUriNode(entity.Uri);
        var pred = graph.CreateUriNode("rdf:type");
        var obj = graph.CreateUriNode(GraphDataType.GetGraphTypeString(entity.GetType()));

        graph.Assert(subj, pred, obj);
    }

    public static void AssertNameTriple(this IGraph graph, NamedEntity entity)
    {
        var subj = graph.CreateUriNode(entity.Uri);
        var pred = graph.CreateUriNode("ddl:hasName");
        var obj = graph.CreateLiteralNode(entity.Name);

        graph.Assert(subj, pred, obj);
    }

    public static void AssertNamedEntityTriple(this IGraph graph, NamedEntity entity)
    {
        graph.AssertNameTriple(entity);
        graph.AssertTypeTriple(entity);
    }
}