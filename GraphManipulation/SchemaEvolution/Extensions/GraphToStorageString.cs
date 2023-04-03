using VDS.RDF;
using VDS.RDF.Writing;
using StringWriter = VDS.RDF.Writing.StringWriter;

namespace GraphManipulation.SchemaEvolution.Extensions;

public static class GraphToStorageString
{
    public static string ToStorageString(this IGraph graph)
    {
        var writer = new CompressingTurtleWriter();

        return StringWriter.Write(graph, writer);
    }
}