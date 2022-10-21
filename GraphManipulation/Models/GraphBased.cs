using GraphManipulation.Interfaces;
using VDS.RDF;

namespace GraphManipulation.Models;

public abstract class GraphBased : IGraphBased
{
    public Uri OntologyNamespace => UriFactory.Create("http://www.cs-22-dt-9-03.org/datastore-description-language#");

    public void AddNamespaces(IGraph graph)
    {
        graph.NamespaceMap.AddNamespace("ddl", OntologyNamespace);
    }

    public GraphBased()
    {
        
    }
    
    // TODO: Lav protected abstract ComputeGraph, og lav en implementering af ToGraph public 
    // TODO: Lav protected abstract ComputeGraphBased, og lav en implementering af FromGraph public
    
    public abstract IGraph ToGraph();
    public abstract IGraphBased FromGraph(IGraph graph);
}

public class GraphBasedException : Exception
{
    public GraphBasedException(string message) : base(message)
    {
    }
}