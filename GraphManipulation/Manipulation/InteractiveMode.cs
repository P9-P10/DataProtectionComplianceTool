using System.Data.SQLite;
using GraphManipulation.Components;
using GraphManipulation.Extensions;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.MetadataManagement;
using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Stores;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace GraphManipulation.Manipulation;

public class InteractiveMode
{
    private readonly ConfigManager _cf;
    private ILogger _logger;

    public InteractiveMode(ConfigManager configManager, ILogger logger)
    {
        _cf = configManager;
        _logger = logger;
    }

    public void Run()
    {
        Console.WriteLine();
        Console.WriteLine(GenerateHashTags(40));
        Console.WriteLine(GenerateSpaces(10) + "Graph Manipulation");
        Console.WriteLine(GenerateHashTags(40));

        var ontologyPath = GetOntologyPath();
        var graphStorageConnectionString = GetGraphStorageConnectionString();

        IGraph ontology = new Graph();
        ontology.LoadFromFile(ontologyPath, new TurtleParser());

        var metadataConnection = new SQLiteConnection();
        const string metadataConnectionString = "";
        metadataConnection.ConnectionString = $"Data Source={metadataConnectionString}";
        var metadataManager = new MetadataManager(metadataConnection, "placeholder_individuals_table");

        var graphStorage = new GraphStorage(graphStorageConnectionString, ontology);

        var stop = false;

        while (!stop)
        {
            var managedDatabases = graphStorage.GetListOfManagedDatabasesWithType();

            var choice = UserInteraction.PresentManagedDatabasesReturnChoice(managedDatabases);

            InitOrManageDatabase(choice, managedDatabases, graphStorage, metadataManager);

            if (UserInteraction.PresentExit("the program"))
            {
                stop = true;
            }
        }
    }

    private string GetOntologyPath()
    {
        var ontologyPath = _cf.GetValue("OntologyPath");

        if (ontologyPath != "")
        {
            return ontologyPath;
        }

        ontologyPath = UserInteraction.GetOntologyPathFromUser();
        _cf.UpdateValue("OntologyPath", ontologyPath);

        return ontologyPath;
    }

    private string GetGraphStorageConnectionString()
    {
        var graphStorageConnectionString = _cf.GetValue("GraphStoragePath");

        if (graphStorageConnectionString != "")
        {
            return graphStorageConnectionString;
        }

        graphStorageConnectionString = UserInteraction.GetGraphStorageConnectionStringFromUser();
        _cf.UpdateValue("GraphStoragePath", graphStorageConnectionString);

        return graphStorageConnectionString;
    }

    private string GetBaseUriFromUser()
    {
        var baseUri = _cf.GetValue("BaseURI");

        if (baseUri != "")
        {
            return baseUri;
        }

        Console.WriteLine();
        Console.WriteLine("Please input the Base Uri for your system: ");
        baseUri = UserInteraction.GetStringFromUser(
            s => !Entity.IsValidUri(s.Trim()),
            "Uri must be valid, try again");

        return baseUri;
    }

    private void InitOrManageDatabase(int choice, List<(string, string)> managedDatabases,
        GraphStorage graphStorage, MetadataManager metadataManager)
    {
        if (choice == 0)
        {
            InitDatabase(graphStorage);
        }
        else
        {
            var chosenTuple = managedDatabases[choice - 1];
            var databaseUri = UriFactory.Create(chosenTuple.Item1);
            var databaseType = GraphDataType.GetTypeFromString(chosenTuple.Item2);

            switch (databaseType)
            {
                case { } when databaseType == typeof(Sqlite):
                    ManageSelectedDatabase<Sqlite>(databaseUri, graphStorage, metadataManager);
                    break;
                default:
                    throw new InteractiveModeException("The type of database is not supported: " + databaseType);
            }
        }
    }

    private void InitDatabase(GraphStorage graphStorage)
    {
        var baseUri = GetBaseUriFromUser();

        Console.WriteLine();
        Console.WriteLine("Please provide the type of database you want to manage: ");
        PresentDatabaseTypes();
        var choice = UserInteraction.GetIntFromUser(
            s => s != 0,
            "Please choose a supported database");

        switch (choice)
        {
            case 0:
                InitSqlite(graphStorage, baseUri);
                break;
            default:
                throw new InteractiveModeException("The chosen type of database is not supported");
        }
    }

    private static void InitSqlite(GraphStorage graphStorage, string baseUri)
    {
        Console.WriteLine();
        Console.WriteLine("Please provide the path to the SQLite: ");
        var flag = false;

        var managedDatabases = graphStorage.GetListOfManagedDatabasesWithType();
        Sqlite sqlite;

        do
        {
            var sqlitePath = UserInteraction.GetStringFromUser(_ => false, "");

            sqlite = new Sqlite("", baseUri, new SQLiteConnection($"Data Source={sqlitePath}"));
            sqlite.BuildFromDataSource();

            if (managedDatabases.Select(tuple => tuple.Item1).FirstOrDefault(s => s == sqlite.Uri.ToString()) != null)
            {
                Console.WriteLine(
                    $"{sqlite.Uri} is already managed by this system, please choose a different database");
            }
            else
            {
                flag = true;
            }
        } while (!flag);


        graphStorage.InitInsert(sqlite);
    }

    private static void PresentDatabaseTypes()
    {
        Console.WriteLine("(0) : SQLite");
    }

    private static void ManageSelectedDatabase<T>(Uri databaseUri, GraphStorage graphStorage,
        MetadataManager metadataManager)
        where T : Database
    {
        var graph = graphStorage.GetLatest(databaseUri);

        var graphManipulator = new Manipulator<T>(graph);

        var stop = false;

        while (!stop)
        {
            Console.WriteLine("Please write manipulation query: ");
            var manipulationQuery = UserInteraction.GetStringFromUser(
                s => !FunctionParser.IsValidManipulationQuery(s),
                "The manipulation query is invalid, please try again");

            try
            {
                FunctionParser.CommandParser(manipulationQuery, graphManipulator, metadataManager);
            }
            catch (ManipulatorException e)
            {
                Console.WriteLine("An error happened, please resolve: " + e.Message);
                continue;
            }

            Console.WriteLine("Commit changes? (y/n)");
            var doCommit = UserInteraction.GetBoolFromUser("y", "n");

            if (!doCommit)
            {
                continue;
            }

            try
            {
                graphStorage.Insert(databaseUri, graphManipulator.Graph, graphManipulator.Changes);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("An error happened: " + e.Message);
                return;
            }

            if (UserInteraction.PresentExit($"manipulating {databaseUri}"))
            {
                stop = true;
            }
        }
    }

    private static string GenerateHashTags(int count)
    {
        return new string('#', count);
    }

    private static string GenerateSpaces(int count)
    {
        return new string(' ', count);
    }
}

public class InteractiveModeException : Exception
{
    public InteractiveModeException(string message) : base(message)
    {
    }
}