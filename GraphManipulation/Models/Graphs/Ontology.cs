using VDS.RDF;

namespace GraphManipulation.Models.Graphs;

public class Ontology : KnowledgeGraph
{
    public Ontology(string path, IRdfReader reader) : base(path, reader)
    {
    }

    public Ontology(StringReader stringReader, string data, IRdfReader reader) : base(stringReader, data, reader)
    {
    }

    protected override void GraphVerification(IGraph graph)
    {
        var subj = graph.CreateUriNode(graph.BaseUri);
        var pred = graph.CreateUriNode(UriFactory.Create(NamespaceMapper.RDF + "type"));
        var obj = graph.CreateUriNode(UriFactory.Create(NamespaceMapper.OWL + "Ontology"));

        var ontologyDefinition = new Triple(subj, pred, obj);

        if (!graph.ContainsTriple(ontologyDefinition))
        {
            throw new OntologyException("Missing ontology definition");
        }
    }
}

public class OntologyException : Exception
{
    public OntologyException(string message) : base(message)
    {
    }
}