using System.Data.SQLite;
using GraphManipulation.Components;
using GraphManipulation.Extensions;
using GraphManipulation.Helpers;
using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Stores;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace GraphManipulation.Manipulation;

public class InteractiveMode
{
    private readonly ConfigManager _cf;

    public InteractiveMode(string configPath)
    {
        _cf = new ConfigManager(configPath);
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

        var graphStorage = new GraphStorage(graphStorageConnectionString, ontology);

        var stop = false;

        while (!stop)
        {
            var managedDatabases = graphStorage.GetListOfManagedDatabasesWithType();

            var choice = PresentManagedDatabasesReturnChoice(managedDatabases);

            InitOrManageDatabase(choice, managedDatabases, graphStorage);

            if (PresentExit("the program"))
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

        ontologyPath = GetOntologyPathFromUser();
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

        graphStorageConnectionString = GetGraphStorageConnectionStringFromUser();
        _cf.UpdateValue("GraphStoragePath", graphStorageConnectionString);

        return graphStorageConnectionString;
    }

    private string GetBaseUriFromUser()
    {
        string baseUri = _cf.GetValue("BaseURI");
        if (baseUri == "")
        {
            Console.WriteLine();
            Console.WriteLine("Please input the Base Uri for your system: ");
            baseUri = GetStringFromUser(
                s => !Entity.IsValidUri(s.Trim()),
                "Uri must be valid, try again");
        }

        return baseUri;
    }

    private string GetOntologyPathFromUser()
    {
        Console.WriteLine();
        Console.WriteLine("Please input the absolute path to your ontology turtle file (.ttl): ");
        var ontologyPath = GetStringFromUser(
            s => !File.Exists(s),
            "Could not find the ontology on the given path, please try again");

        return ontologyPath;
    }

    private string GetGraphStorageConnectionStringFromUser()
    {
        Console.WriteLine();
        Console.WriteLine("Please input the absolute path to your GraphStorage (.sqlite): ");
        var graphStoragePath = GetStringFromUser(
            s =>
            {
                var suffixIsCorrect = s.Contains(".sqlite");

                if (suffixIsCorrect && !File.Exists(s))
                {
                    Console.WriteLine($"Creating file at: {s}");
                }

                return !s.Contains(".sqlite");
            },
            "Path must include the .sqlite suffix");

        return graphStoragePath;
    }

    private string GetStringFromUser(Func<string, bool> errorPredicate, string errorMessage)
    {
        string result;
        var flag = false;

        do
        {
            result = Console.ReadLine()!;

            if (errorPredicate(result))
            {
                Console.WriteLine(errorMessage);
            }
            else
            {
                flag = true;
            }
        } while (!flag);

        return result;
    }

    private int GetIntFromUser(Func<int, bool> errorPredicate, string errorMessage)
    {
        int result;
        var flag = false;

        do
        {
            var numberString = GetStringFromUser(
                s => !int.TryParse(s, out _),
                "Input cannot be converted to an integer, please try again");

            int.TryParse(numberString, out result);

            if (errorPredicate(result))
            {
                Console.WriteLine(errorMessage);
            }
            else
            {
                flag = true;
            }
        } while (!flag);

        return result;
    }

    private bool GetBoolFromUser(string trueEquivalent, string falseEquivalent)
    {
        var boolString = GetStringFromUser(
            s =>
                !string.Equals(s, trueEquivalent, StringComparison.CurrentCultureIgnoreCase)
                &&
                !string.Equals(s, falseEquivalent, StringComparison.CurrentCultureIgnoreCase),
            "Input cannot be converted be parsed, please try again");

        return boolString == trueEquivalent;
    }

    private int PresentManagedDatabasesReturnChoice(List<(string, string)> managedDatabases)
    {
        Console.WriteLine();
        Console.WriteLine("These are the currently managed databases: ");

        var index = 1;

        managedDatabases.ForEach(tuple => Console.WriteLine($"({index++}) : {tuple.Item1} - {tuple.Item2}"));
        Console.WriteLine("\n(0) - Insert new database");

        Console.WriteLine();
        Console.WriteLine("Please select which database you want to manipulate: ");
        var choice = GetIntFromUser(
            i => i < 0 || i > managedDatabases.Count,
            $"Please choose a number between 0 and {managedDatabases.Count} (inclusive)");

        return choice;
    }

    private bool PresentExit(string exitFrom)
    {
        Console.WriteLine();
        Console.WriteLine($"Do you want to exit from {exitFrom}? (y/n)");
        return GetBoolFromUser("y", "n");
    }

    private void InitOrManageDatabase(int choice, List<(string, string)> managedDatabases,
        GraphStorage graphStorage)
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
                    ManageSelectedDatabase<Sqlite>(databaseUri, graphStorage);
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
        var choice = GetIntFromUser(
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

    private void InitSqlite(GraphStorage graphStorage, string baseUri)
    {
        Console.WriteLine();
        Console.WriteLine("Please provide the path to the SQLite: ");
        var flag = false;

        var managedDatabases = graphStorage.GetListOfManagedDatabasesWithType();
        Sqlite sqlite;

        do
        {
            var sqlitePath = GetStringFromUser(_ => false, "");

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

    private void ManageSelectedDatabase<T>(Uri databaseUri, GraphStorage graphStorage) where T : Database
    {
        var graph = graphStorage.GetLatest(databaseUri);

        var graphManipulator = new Manipulator<T>(graph);

        var stop = false;

        while (!stop)
        {
            Console.WriteLine("Please write manipulation query: ");
            var manipulationQuery = GetStringFromUser(
                s => !FunctionParser.IsValidManipulationQuery(s),
                "The manipulation query is invalid, please try again");

            try
            {
                FunctionParser.CommandParser(manipulationQuery, graphManipulator);
            }
            catch (ManipulatorException e)
            {
                Console.WriteLine("An error happened, please resolve: " + e.Message);
                continue;
            }

            Console.WriteLine("Commit changes? (y/n)");
            var doCommit = GetBoolFromUser("y", "n");

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

            if (PresentExit($"manipulating {databaseUri}"))
            {
                stop = true;
            }
        }
    }

    private static string GenerateHashTags(int count)
    {
        return new('#', count);
    }

    private static string GenerateSpaces(int count)
    {
        return new(' ', count);
    }
}

public class InteractiveModeException : Exception
{
    public InteractiveModeException(string message) : base(message)
    {
    }
}