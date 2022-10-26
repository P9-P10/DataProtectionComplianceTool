using GraphManipulation.Interfaces;
using VDS.RDF;

namespace GraphManipulation.Models;

public abstract class GraphBased
{
    public Uri OntologyNamespace => UriFactory.Create("http://www.cs-22-dt-9-03.org/datastore-description-language#");

    public GraphBased()
    {
        
    }

    public virtual IGraph ToGraph()
    {
        IGraph graph = new Graph();
        AddNamespaces(graph);

        return graph;
    }
    
    private void AddNamespaces(IGraph graph)
    {
        graph.NamespaceMap.AddNamespace("ddl", OntologyNamespace);
    }

    public virtual GraphBased FromGraph(IGraph graph)
    {
        throw new NotImplementedException();
    }
}

public class GraphBasedException : Exception
{
    public GraphBasedException(string message) : base(message)
    {
    }
}