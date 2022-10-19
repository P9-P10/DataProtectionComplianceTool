using VDS.RDF;

namespace GraphManipulation;

public class Ontology
{
    private readonly string? _path;
    private readonly TextReader? _textReader;
    private readonly IRdfReader _ontologyReader;

    public Ontology(string path, IRdfReader reader)
    {
        _path = path;
        _ontologyReader = reader;
    }

    public Ontology(TextReader textReader, IRdfReader reader)
    {
        _textReader = textReader;
        _ontologyReader = reader;
    }

    public IGraph ToGraph()
    {
        IGraph resultGraph = new Graph();

        if (_path is not null)
        {
            _ontologyReader.Load(resultGraph, _path);
        }
        else if (_textReader is not null)
        {
            _ontologyReader.Load(resultGraph, _textReader);
        }

        if (resultGraph.BaseUri is null)
        {
            throw new OntologyException("No base defined");
        }

        IUriNode subj = resultGraph.CreateUriNode(resultGraph.BaseUri);
        IUriNode pred = resultGraph.CreateUriNode(UriFactory.Create(NamespaceMapper.RDF + "type"));
        IUriNode obj = resultGraph.CreateUriNode(UriFactory.Create( NamespaceMapper.OWL + "Ontology"));

        Triple ontologyDefinition = new Triple(subj, pred, obj);

        if (!resultGraph.ContainsTriple(ontologyDefinition))
        {
            throw new OntologyException("Missing ontology definition");
        }
        
        return resultGraph;
    }
}

public class OntologyException : Exception
{
    public OntologyException() : base() {}
    public OntologyException(string message) : base(message) { }
}