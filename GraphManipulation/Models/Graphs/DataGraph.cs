using VDS.RDF;
using VDS.RDF.Shacl;

namespace GraphManipulation.Models.Graphs;

public class DataGraph : KnowledgeGraph
{
    private readonly List<ShapesGraph> _shapesGraphs = new();

    public DataGraph(string path, IRdfReader reader) : base(path, reader)
    {
    }

    public DataGraph(StringReader stringReader, string data, IRdfReader reader) : base(stringReader, data, reader)
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
            .Where(report => !report.Conforms)
            .ToList();

        if (nonconformity.Count != 0)
        {
            var message = "";

            foreach (var report in nonconformity)
            {
                message += "\nConforms: " + report.Conforms + " (" + report.Results.Count + ")";

                foreach (var result in report.Results)
                {
                    message += result.FocusNode is not null ? "\nFocus node: " + result.FocusNode.ToString() : "";
                    message += result.ResultPath is not null ? "\nResult path: " + result.ResultPath : "";
                    message += result.ResultValue is not null ? "\nResult value: " + result.ResultValue.ToString() : "";
                    message += "\nMessage: " + result.Message.Value + "\n";
                }
            }

            Console.WriteLine(message);
            throw new DataGraphException(message);
        }
    }
}

public class DataGraphException : Exception
{
    public DataGraphException()
    {
    }

    public DataGraphException(string message) : base(message)
    {
    }
}