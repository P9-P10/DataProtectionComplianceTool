using VDS.RDF;
using VDS.RDF.Shacl;

namespace GraphManipulation.Models;

public class DataGraph : KnowledgeGraph
{
    private readonly List<ShapesGraph> _shapesGraphs = new();
    
    public DataGraph(string path, IRdfReader reader) : base(path, reader)
    {
    }

    public DataGraph(TextReader textReader, IRdfReader reader) : base(textReader, reader)
    {
    }

    public void AddShapesGraph(ShapesGraph shapesGraph)
    {
        _shapesGraphs.Add(shapesGraph);
    }

    protected override void GraphVerification(IGraph graph)
    {
        // https://github.com/dotnetrdf/dotnetrdf/blob/maintenance/2.x/Testing/unittest/Shacl/Examples.cs
        var nonconformity = _shapesGraphs
            .Select(shapesGraph => shapesGraph.Validate(graph))
            .Where(result => !result.Conforms)
            .ToList();

        if (nonconformity.Count != 0)
        {
            throw new DataGraphException("Datagraph does not conform: " + nonconformity);
        }
    }
}


public class DataGraphException : Exception
{
    public DataGraphException() : base()
    {
    }

    public DataGraphException(string message) : base(message)
    {
    }
}