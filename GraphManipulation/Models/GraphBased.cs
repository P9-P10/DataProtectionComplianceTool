using VDS.RDF;

namespace GraphManipulation.Models;

public abstract class GraphBased
{
    // TODO: Maybe do this? Problemet er at eksekveringen jo også skal stoppe hvis der f.eks. mangler data
    // protected List<Violation> Violations = new();
    //
    // public virtual List<Violation> GetViolations()
    // {
    //     return Violations;
    // }

    private static Uri OntologyNamespace =>
        UriFactory.Create("http://www.cs-22-dt-9-03.org/datastore-description-language#");


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