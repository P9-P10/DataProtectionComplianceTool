// See https://aka.ms/new-console-template for more information

using System.Data.SQLite;
using GraphManipulation.Extensions;
using GraphManipulation.Models.Stores;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Shacl;
using VDS.RDF.Shacl.Validation;
using VDS.RDF.Writing;

namespace GraphManipulation;

public static class Program
{
    public static void Main()
    {
        CreateAndValidateGraph();
    }

    private static void CreateAndValidateGraph()
    {
        string baseUri = "http://www.test.com/";

        // string database = "OptimizedAdvancedDatabase.sqlite";
        string database = "SimpleDatabase.sqlite";

        using var conn = new SQLiteConnection($"Data Source=/home/ane/Documents/GitHub/Legeplads/Databases/{database}");

        var sqlite = new Sqlite("", baseUri, conn);

        sqlite.Build();

        IGraph graph = sqlite.ToGraph();

        var writer = new CompressingTurtleWriter();
        const string path = "/home/ane/Documents/GitHub/GraphManipulation/GraphManipulation/test.ttl";
        writer.Save(graph, path);

        IGraph dataGraph = new Graph();
        dataGraph.LoadFromFile(path);

        IGraph ontology = new Graph();
        const string ontologyPath = "/home/ane/Documents/GitHub/GraphManipulation/GraphManipulation/Ontologies/datastore-description-language.ttl";
        ontology.LoadFromFile(ontologyPath, new TurtleParser());
        var shapesGraph = new ShapesGraph(ontology);

        PrintReport(shapesGraph.Validate(dataGraph));
        
        Console.WriteLine(sqlite.ToSqlCreateStatement());
    }

    private static void PrintReport(Report report)
    {
        var message = "";

        message += "\nConforms: " + report.Conforms + (report.Results.Count == 0 ? "" : " (" + report.Results.Count + ")");

        foreach (var result in report.Results)
        {
            message += result.FocusNode is not null ? "\nFocus node: " + result.FocusNode.ToString() : "";
            message += result.ResultPath is not null ? "\nResult path: " + result.ResultPath : "";
            message += result.ResultValue is not null ? "\nResult value: " + result.ResultValue.ToString() : "";
            message += "\nMessage: " + result.Message.Value + "\n";
        }

        Console.WriteLine(message);
    }
}