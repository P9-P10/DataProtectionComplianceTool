using VDS.RDF;

namespace GraphManipulation.Models;

public class Ontology : KnowledgeGraph
{
    public Ontology(string? path, IRdfReader reader) : base(path, reader)
    {
    }

    public Ontology(TextReader textReader, IRdfReader reader) : base(textReader, reader)
    {
    }
    
    protected override void GraphVerification(IGraph graph)
    {
        IUriNode subj = graph.CreateUriNode(graph.BaseUri);
        IUriNode pred = graph.CreateUriNode(UriFactory.Create(NamespaceMapper.RDF + "type"));
        IUriNode obj = graph.CreateUriNode(UriFactory.Create(NamespaceMapper.OWL + "Ontology"));

        Triple ontologyDefinition = new Triple(subj, pred, obj);

        if (!graph.ContainsTriple(ontologyDefinition))
        {
            throw new OntologyException("Missing ontology definition");
        }
    }
}

public class OntologyException : Exception
{
    public OntologyException() : base()
    {
    }

    public OntologyException(string message) : base(message)
    {
    }
}