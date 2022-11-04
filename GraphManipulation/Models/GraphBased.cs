using VDS.RDF;

namespace GraphManipulation.Models;

public abstract class GraphBased
{
    public static Uri OntologyNamespace =>
        UriFactory.Create("http://www.cs-22-dt-9-03.org/datastore-description-language#");

    public virtual IGraph ToGraph()
    {
        IGraph graph = new Graph();

        AddNamespaces(graph);

        return graph;
    }

    private static void AddNamespaces(IGraph graph)
    {
        graph.NamespaceMap.AddNamespace("ddl", OntologyNamespace);
    }

    public virtual void FromGraph(IGraph graph)
    {
        
    }
}

public class GraphBasedException : Exception
{
    public GraphBasedException(string message) : base(message)
    {
    }
}