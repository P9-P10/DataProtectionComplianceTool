// See https://aka.ms/new-console-template for more information

using System.Data.SQLite;
using GraphManipulation.Extensions;
using GraphManipulation.Models.Stores;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Datasets;
using VDS.RDF.Writing;

namespace GraphManipulation;

public static class Program
{
    private const string File = "output.ttl";
    private const string Path = $"/home/ane/Documents/GitHub/GraphManipulation/GraphManipulation/{File}";
    
    public static void Main()
    {
        // CreateAndValidateGraph();
        SparqlExperiment();
    }

    private static void SparqlExperiment()
    {
        // https://dotnetrdf.org/docs/stable/user_guide/Querying-With-SPARQL.html
        var graph = new Graph();
        graph.LoadFromFile(Path);
        
        var parser = new SparqlQueryParser();
        var query = parser.ParseFromString("SELECT ?relational WHERE { ?relational a ddl:Relational }");
        
        TripleStore tripleStore = new TripleStore();
        tripleStore.Add(graph);
        
        InMemoryDataset dataset = new InMemoryDataset(tripleStore);
        
        var processor = new LeviathanQueryProcessor(dataset);
        
        var results = processor.ProcessQuery(query);

        int a = 3;
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
        
        writer.Save(graph, Path);

        IGraph dataGraph = new Graph();
        dataGraph.LoadFromFile(Path);

        IGraph ontology = new Graph();
        const string ontologyPath =
            "/home/ane/Documents/GitHub/GraphManipulation/GraphManipulation/Ontologies/datastore-description-language.ttl";
        ontology.LoadFromFile(ontologyPath, new TurtleParser());

        var report = dataGraph.ValidateUsing(ontology);

        Validation.PrintValidationReport(report);

        // Console.WriteLine(sqlite.ToSqlCreateStatement());
    }
}