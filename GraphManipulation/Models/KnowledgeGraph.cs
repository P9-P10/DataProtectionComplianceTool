using VDS.RDF;

namespace GraphManipulation.Models;

public abstract class KnowledgeGraph
{
    private readonly string? _path;
    private readonly TextReader? _textReader;
    private readonly IRdfReader _graphReader;

    protected KnowledgeGraph(string path, IRdfReader reader)
    {
        _path = path;
        _graphReader = reader;
    }

    protected KnowledgeGraph(TextReader textReader, IRdfReader reader)
    {
        _textReader = textReader;
        _graphReader = reader;
    }

    protected abstract void GraphVerification(IGraph graph);

    public IGraph ToGraph()
    {
        IGraph resultGraph = new Graph();

        if (_path is not null)
        {
            _graphReader.Load(resultGraph, _path);
        }
        else if (_textReader is not null)
        {
            _graphReader.Load(resultGraph, _textReader);
        }

        if (resultGraph.BaseUri is null)
        {
            throw new KnowledgeGraphException("No base defined");
        }
        
        GraphVerification(resultGraph);

        return resultGraph;
    }
}

public class KnowledgeGraphException : Exception
{
    public KnowledgeGraphException(string message) : base(message) { }
}