// See https://aka.ms/new-console-template for more information

using System.Data.SQLite;
using GraphManipulation.Extensions;
using GraphManipulation.Models.Stores;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query.Inference;
using VDS.RDF.Shacl;
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
        var baseUri = "http://www.test.com/";

        var database = "OptimizedAdvancedDatabase.sqlite";
        // string database = "SimpleDatabase.sqlite";

        using var conn = new SQLiteConnection($"Data Source=/home/ane/Documents/GitHub/Legeplads/Databases/{database}");

        var sqlite = new Sqlite("", baseUri, conn);

        sqlite.BuildFromDataSource();

        var graph = sqlite.ToGraph();

        var writer = new CompressingTurtleWriter();
        const string file = "output.ttl";
        const string path = $"/home/ane/Documents/GitHub/GraphManipulation/GraphManipulation/{file}";
        writer.Save(graph, path);

        IGraph dataGraph = new Graph();
        dataGraph.LoadFromFile(path);


        IGraph ontology = new Graph();
        const string ontologyPath =
            "/home/ane/Documents/GitHub/GraphManipulation/GraphManipulation/Ontologies/datastore-description-language.ttl";
        ontology.LoadFromFile(ontologyPath, new TurtleParser());

        var report = dataGraph.ValidateUsing(ontology);

        Validation.PrintValidationReport(report);

        // Console.WriteLine(sqlite.ToSqlCreateStatement());
    }
}