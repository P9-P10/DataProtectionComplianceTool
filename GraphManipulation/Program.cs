// See https://aka.ms/new-console-template for more information

using System.Data.SQLite;
using GraphManipulation.Components;
using GraphManipulation.Extensions;
using GraphManipulation.Helpers;
using GraphManipulation.Manipulation;
using GraphManipulation.Models.Stores;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Datasets;
using VDS.RDF.Writing;
using StringWriter = VDS.RDF.Writing.StringWriter;

namespace GraphManipulation;

public static class Program
{
    private static ConfigManager cf = new("C:/Users/Alexander N/GitHub/GraphManipulation/config.json");
    
    //private const string GraphStoragePath =
       // "/home/ane/Documents/GitHub/GraphManipulation/GraphManipulation/GraphStorage.sqlite";

    //private const string OptimizedDatabaseName = "OptimizedAdvancedDatabase.sqlite";
    //private const string SimpleDatabaseName = "SimpleDatabase.sqlite";

    //private const string OptimizedDatabasePath =
      //  $"/home/ane/Documents/GitHub/Legeplads/Databases/{OptimizedDatabaseName}";

    //private const string SimpleDatabasePath = $"/home/ane/Documents/GitHub/Legeplads/Databases/{SimpleDatabaseName}";
    //private const string BaseUri = "http://www.test.com/";
    //private const string OutputFileName = "output.ttl";

    //private const string OutputPath =
     //   $"/home/ane/Documents/GitHub/GraphManipulation/GraphManipulation/{OutputFileName}";

    //private const string OntologyPath =
    //    "/home/ane/Documents/GitHub/GraphManipulation/GraphManipulation/Ontologies/database-description-language.ttl";

    public static void Main()
    {
        // Console.WriteLine();
        // var arguments = Environment.GetCommandLineArgs();
        // Console.WriteLine(string.Join(", ", arguments));

        // SparqlExperiment();
        // SparqlExperiment("SELECT * WHERE { ?s ?p ?o }");
        // SparqlExperiment(@"SELECT ?name ?o WHERE { ?database a ddl:Database . ?database ddl:hasName ?name . ?database ?p ?o }");
        // SparqlExperiment("SELECT ?something ?name WHERE { ?something a ddl:Column . ?something ddl:Database ?name }");

        //CreateAndValidateGraph();
        // WorkingWithGraphStorage();

        // InitGraphStorage();
        //MakeChangeToGraph();

        Interactive();
    }

    private static void Interactive()
    {
        var interactive = new InteractiveMode();
        InteractiveMode.Run();
    }

    private static void InitGraphStorage()
    {
        IGraph ontology = new Graph();
        ontology.LoadFromFile(cf.getValue("OntologyPath"), new TurtleParser());

        var graphStorage = new GraphStorage(cf.getValue("GraphStoragePath"), ontology, true);
    }

    private static void MakeChangeToGraph()
    {
        IGraph ontology = new Graph();
        ontology.LoadFromFile(cf.getValue("OntologyPath"), new TurtleParser());

        var graphStorage = new GraphStorage(cf.getValue("GraphStoragePath"), ontology);

        using var simpleConn = new SQLiteConnection($"Data Source={cf.getValue("DatabasePath")+cf.getValue("SimpleDatabaseName")}");
        var simpleSqlite = new Sqlite("", cf.getValue("BaseURI"), simpleConn);
        simpleSqlite.BuildFromDataSource();

        var dataGraph = graphStorage.GetLatest(simpleSqlite);

        var graphManipulator = new Manipulator<Sqlite>(dataGraph);

        var userDataTable = simpleSqlite
            .FindSchema("main")!
            .FindTable("UserData")!;

        var emailColumn = simpleSqlite
            .FindSchema("main")!
            .FindTable("Users")!
            .FindColumn("email")!;

        var columnUri = emailColumn.Uri.ToString();

        userDataTable.AddStructure(emailColumn);

        graphManipulator.Move(new Uri(columnUri), emailColumn.Uri);

        graphStorage.Insert(simpleSqlite, graphManipulator.Graph, graphManipulator.Changes);
    }

    private static void WorkingWithGraphStorage()
    {
        IGraph ontology = new Graph();
        ontology.LoadFromFile(cf.getValue("OntologyPath"), new TurtleParser());

        var graphStorage = new GraphStorage(cf.getValue("GraphStoragePath"), ontology);

        using var simpleConn = new SQLiteConnection($"Data Source={cf.getValue("DatabasePath")+cf.getValue("SimpleDatabaseName")}");
        var simpleSqlite = new Sqlite("", cf.getValue("BaseURI"), simpleConn);
        simpleSqlite.BuildFromDataSource();
        var simpleGraph = simpleSqlite.ToGraph();

        var graphManipulator = new Manipulator<Sqlite>(simpleGraph);

        var userDataTable = simpleSqlite
            .FindSchema("main")!
            .FindTable("UserData")!;

        var emailColumn = simpleSqlite
            .FindSchema("main")!
            .FindTable("Users")!
            .FindColumn("email")!;

        var columnUri = emailColumn.Uri.ToString();

        userDataTable.AddStructure(emailColumn);

        graphManipulator.Move(new Uri(columnUri), emailColumn.Uri);

        graphStorage.Insert(simpleSqlite, graphManipulator.Graph, graphManipulator.Changes);

        var dataGraph = graphStorage.GetLatest(simpleSqlite);

        var writer = new CompressingTurtleWriter();

        Console.WriteLine(StringWriter.Write(dataGraph, writer));
        Console.WriteLine(simpleSqlite.ToSqlCreateStatement());
    }

    private static void SparqlExperiment( /* string commandText */)
    {
        var graph = new Graph();
        graph.LoadFromFile(cf.getValue("OutputPath"));

        IGraph ontology = new Graph();
        ontology.LoadFromFile(cf.getValue("OntologyPath"), new TurtleParser());

        graph.ValidateUsing(ontology);

        // var queryString = new SparqlParameterizedString();
        // queryString.Namespaces.AddNamespace(
        //     DatabaseDescriptionLanguage.OntologyPrefix, 
        //     DatabaseDescriptionLanguage.OntologyUri);
        // queryString.CommandText = commandText;


        var parser = new SparqlQueryParser();
        // var query = parser.ParseFromString(queryString);
        var query = parser.ParseFromFile(
            "/home/ane/Documents/GitHub/GraphManipulation/GraphManipulation/sparqlQuery.rq");

        var tripleStore = new TripleStore();
        tripleStore.Add(graph);

        var dataset = new InMemoryDataset(tripleStore);

        var processor = new LeviathanQueryProcessor(dataset);

        var results = (processor.ProcessQuery(query) as SparqlResultSet)!;

        foreach (var result in results)
            Console.WriteLine(result);
    }

    private static void CreateAndValidateGraph()
    {
        using var optimizedConn = new SQLiteConnection($"Data Source={cf.getValue("DatabasePath")+cf.getValue("OptimizedDatabaseName")}");
        using var simpleConn = new SQLiteConnection($"Data Source={cf.getValue("DatabasePath")+cf.getValue("SimpleDatabaseName")}");

        var optimizedSqlite = new Sqlite("", cf.getValue("BaseURI"), optimizedConn);
        // var simpleSqlite = new Sqlite("", BaseUri, simpleConn);

        optimizedSqlite.BuildFromDataSource();
        // simpleSqlite.BuildFromDataSource();

        var optimizedGraph = optimizedSqlite.ToGraph();
        // var simpleGraph = simpleSqlite.ToGraph();

        var combinedGraph = new Graph();
        combinedGraph.Merge(optimizedGraph);
        // combinedGraph.Merge(simpleGraph);

        var writer = new CompressingTurtleWriter();

        writer.Save(combinedGraph, cf.getValue("OutputPath"));

        IGraph dataGraph = new Graph();
        dataGraph.LoadFromFile(cf.getValue("OutputPath"));

        IGraph ontology = new Graph();
        ontology.LoadFromFile(cf.getValue("OntologyPath"), new TurtleParser());

        var report = dataGraph.ValidateUsing(ontology);

        GraphValidation.PrintValidationReport(report);
        // Console.WriteLine(simpleSqlite.ToSqlCreateStatement());
    }
}