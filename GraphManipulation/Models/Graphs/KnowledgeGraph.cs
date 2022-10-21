using VDS.RDF;

namespace GraphManipulation.Models.Graphs;

public abstract class KnowledgeGraph
{
    private readonly string? _dataString;
    private readonly IRdfReader _graphReader;
    private readonly string? _path;
    private StringReader? _stringReader;

    protected KnowledgeGraph(string path, IRdfReader reader)
    {
        _path = path;
        _graphReader = reader;
    }

    protected KnowledgeGraph(StringReader stringReader, string data, IRdfReader reader)
    {
        _stringReader = stringReader;
        _dataString = data;
        _graphReader = reader;
    }
    
    // TODO: Det ville måske også give mening at man kan lave en KnowledgeGraph ud fra en IGraph

    protected abstract void GraphVerification(IGraph graph);

    public IGraph ToGraph()
    {
        IGraph resultGraph = new Graph();

        if (_path is not null)
        {
            _graphReader.Load(resultGraph, _path);
        }
        else if (_stringReader is not null)
        {
            _stringReader = new StringReader(_dataString!);
            _graphReader.Load(resultGraph, _stringReader);
        }

        if (resultGraph.BaseUri is null) throw new KnowledgeGraphException("No base defined");

        GraphVerification(resultGraph);

        return resultGraph;
    }
}

public class KnowledgeGraphException : Exception
{
    public KnowledgeGraphException(string message) : base(message)
    {
    }
}