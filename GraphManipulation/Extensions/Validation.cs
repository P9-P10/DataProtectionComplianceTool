using GraphManipulation.Models.Stores;
using VDS.RDF;
using VDS.RDF.Query.Inference;
using VDS.RDF.Shacl;
using VDS.RDF.Shacl.Validation;

namespace GraphManipulation.Extensions;

public static class Validation
{
    public static Report ValidateUsing(this DataStore dataStore, IGraph ontology)
    {
        return dataStore.ToGraph().ValidateUsing(ontology);
    }
    
    public static Report ValidateUsing(this IGraph graph, IGraph ontology)
    {
        var shapesGraph = new ShapesGraph(ontology);

        var reasoner = new StaticRdfsReasoner();
        reasoner.Initialise(ontology);
        reasoner.Apply(graph);

        return shapesGraph.Validate(graph);
    }
    
    public static string PrintValidationReport(Report report)
    {
        var message = "";

        message += "\nConforms: " + report.Conforms +
                   (report.Results.Count == 0 ? "" : " (" + report.Results.Count + ")");

        foreach (var result in report.Results)
        {
            message += result.FocusNode is not null ? "\nFocus node: " + result.FocusNode.ToString() : "";
            message += result.ResultPath is not null ? "\nResult path: " + result.ResultPath : "";
            message += result.ResultValue is not null ? "\nResult value: " + result.ResultValue.ToString() : "";
            message += "\nMessage: " + result.Message.Value + "\n";
        }

        Console.WriteLine(message);
        return message;
    }
}