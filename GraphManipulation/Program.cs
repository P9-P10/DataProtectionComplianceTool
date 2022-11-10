// See https://aka.ms/new-console-template for more information

using System.Data.SQLite;
using GraphManipulation.Extensions;
using GraphManipulation.Models.Stores;
using GraphManipulation.Ontologies;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Datasets;
using VDS.RDF.Writing;

namespace GraphManipulation;

public static class Program
{
    private const string BaseUri = "http://www.test.com/";
    private const string File = "output.ttl";
    private const string Path = $"/home/ane/Documents/GitHub/GraphManipulation/GraphManipulation/{File}";
    private const string OntologyPath =
        "/home/ane/Documents/GitHub/GraphManipulation/GraphManipulation/Ontologies/datastore-description-language.ttl";
    
    public static void Main()
    {
        CreateAndValidateGraph();
        SparqlExperiment("SELECT ?something WHERE { ?something a ddl:Column }");
    }

    private static void SparqlExperiment(string commandText)
    {
        // https://dotnetrdf.org/docs/stable/user_guide/Querying-With-SPARQL.html
        var graph = new Graph();
        graph.LoadFromFile(Path);
        
        IGraph ontology = new Graph();
        ontology.LoadFromFile(OntologyPath, new TurtleParser());

        graph.ValidateUsing(ontology);

        var queryString = new SparqlParameterizedString();
        queryString.Namespaces.AddNamespace("ddl", DataStoreDescriptionLanguage.OntologyUri);
        queryString.CommandText = commandText;
        
        
        var parser = new SparqlQueryParser();
        var query = parser.ParseFromString(queryString);
        
        var tripleStore = new TripleStore();
        tripleStore.Add(graph);
        
        var dataset = new InMemoryDataset(tripleStore);
        
        var processor = new LeviathanQueryProcessor(dataset);
        
        var results = (processor.ProcessQuery(query) as SparqlResultSet)!;

        foreach (var result in results)
        {
            Console.WriteLine(result);
        }
    }

    private static void CreateAndValidateGraph()
    {
        var database = "OptimizedAdvancedDatabase.sqlite";
        // string database = "SimpleDatabase.sqlite";

        using var conn = new SQLiteConnection($"Data Source=/home/ane/Documents/GitHub/Legeplads/Databases/{database}");

        var sqlite = new Sqlite("", BaseUri, conn);

        sqlite.BuildFromDataSource();

        var graph = sqlite.ToGraph();

        var writer = new CompressingTurtleWriter();
        
        writer.Save(graph, Path);

        IGraph dataGraph = new Graph();
        dataGraph.LoadFromFile(Path);

        IGraph ontology = new Graph();
        ontology.LoadFromFile(OntologyPath, new TurtleParser());

        var report = dataGraph.ValidateUsing(ontology);

        Validation.PrintValidationReport(report);
    }
}