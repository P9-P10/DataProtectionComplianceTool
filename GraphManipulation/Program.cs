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
    private static readonly string ProjectFolder =
        Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

    private static readonly string FilePath = Path.Combine(ProjectFolder, "config.json");
    private static readonly ConfigManager Cf = new(FilePath);

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
        InteractiveMode.Run(FilePath);
    }

    private static void InitGraphStorage()
    {
        IGraph ontology = new Graph();
        ontology.LoadFromFile(Cf.GetValue("OntologyPath"), new TurtleParser());

        var graphStorage = new GraphStorage(Cf.GetValue("GraphStoragePath"), ontology, true);
    }

    private static void MakeChangeToGraph()
    {
        IGraph ontology = new Graph();
        ontology.LoadFromFile(Cf.GetValue("OntologyPath"), new TurtleParser());

        var graphStorage = new GraphStorage(Cf.GetValue("GraphStoragePath"), ontology);

        using var simpleConn =
            new SQLiteConnection($"Data Source={Cf.GetValue("DatabasePath") + Cf.GetValue("SimpleDatabaseName")}");
        var simpleSqlite = new Sqlite("", Cf.GetValue("BaseURI"), simpleConn);
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
        ontology.LoadFromFile(Cf.GetValue("OntologyPath"), new TurtleParser());

        var graphStorage = new GraphStorage(Cf.GetValue("GraphStoragePath"), ontology);

        using var simpleConn =
            new SQLiteConnection($"Data Source={Cf.GetValue("DatabasePath") + Cf.GetValue("SimpleDatabaseName")}");
        var simpleSqlite = new Sqlite("", Cf.GetValue("BaseURI"), simpleConn);
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
        graph.LoadFromFile(Cf.GetValue("OutputPath"));

        IGraph ontology = new Graph();
        ontology.LoadFromFile(Cf.GetValue("OntologyPath"), new TurtleParser());

        graph.ValidateUsing(ontology);

        // var queryString = new SparqlParameterizedString();
        // queryString.Namespaces.AddNamespace(
        //     DatabaseDescriptionLanguage.OntologyPrefix, 
        //     DatabaseDescriptionLanguage.OntologyUri);
        // queryString.CommandText = commandText;


        var parser = new SparqlQueryParser();
        // var query = parser.ParseFromString(queryString);
        var query = parser.ParseFromFile(Path.Combine(ProjectFolder, "GraphManipulation/sparqlQuery.rq"));

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
        using var optimizedConn =
            new SQLiteConnection($"Data Source={Cf.GetValue("DatabasePath") + Cf.GetValue("OptimizedDatabaseName")}");
        using var simpleConn =
            new SQLiteConnection($"Data Source={Cf.GetValue("DatabasePath") + Cf.GetValue("SimpleDatabaseName")}");

        var optimizedSqlite = new Sqlite("", Cf.GetValue("BaseURI"), optimizedConn);
        // var simpleSqlite = new Sqlite("", BaseUri, simpleConn);

        optimizedSqlite.BuildFromDataSource();
        // simpleSqlite.BuildFromDataSource();

        var optimizedGraph = optimizedSqlite.ToGraph();
        // var simpleGraph = simpleSqlite.ToGraph();

        var combinedGraph = new Graph();
        combinedGraph.Merge(optimizedGraph);
        // combinedGraph.Merge(simpleGraph);

        var writer = new CompressingTurtleWriter();

        writer.Save(combinedGraph, Cf.GetValue("OutputPath"));

        IGraph dataGraph = new Graph();
        dataGraph.LoadFromFile(Cf.GetValue("OutputPath"));

        IGraph ontology = new Graph();
        ontology.LoadFromFile(Cf.GetValue("OntologyPath"), new TurtleParser());

        var report = dataGraph.ValidateUsing(ontology);

        GraphValidation.PrintValidationReport(report);
        // Console.WriteLine(simpleSqlite.ToSqlCreateStatement());
    }
}