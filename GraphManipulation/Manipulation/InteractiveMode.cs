using System.Data.SQLite;
using GraphManipulation.Extensions;
using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Stores;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace GraphManipulation.Manipulation;

public class InteractiveMode
{
    // /home/ane/Documents/GitHub/GraphManipulation/GraphManipulation/Ontologies/datastore-description-language.ttl
    // /home/ane/Documents/GitHub/GraphManipulation/GraphManipulation/GraphStorage.sqlite
    // MOVE(http://www.test.com/SimpleDatabase/main/UserData/email, http://www.test.com/SimpleDatabase/main/Users/email)
    // MOVE(http://www.test.com/SimpleDatabase/main/Users/email, http://www.test.com/SimpleDatabase/main/UserData/email)

    public void Run()
    {
        Console.WriteLine();
        Console.WriteLine("Welcome to this interactive experience!");


        var ontologyPath = GetOntologyPathFromUser();
        var graphStoragePath = GetGraphStoragePathFromUser();

        IGraph ontology = new Graph();
        ontology.LoadFromFile(ontologyPath, new TurtleParser());

        var graphStorage = new GraphStorage(graphStoragePath, ontology);

        var stop = false;

        while (!stop)
        {
            var managedDataStores = graphStorage.GetListOfManagedDataStoresWithType();

            var choice = PresentManagedDataStoresResultingInChoice(managedDataStores);

            InitOrManageDataStore(choice, managedDataStores, graphStorage);

            if (PresentExit("the program"))
            {
                stop = true;
            }
        }
    }

    private static string GetBaseUriFromUser()
    {
        Console.WriteLine();
        Console.WriteLine("Please input the Base Uri for your system: ");
        var baseUri = GetStringFromUser(
            s => !Entity.IsValidUri(s.Trim()),
            "Uri must be valid, try again");

        return baseUri;
    }

    private static string GetOntologyPathFromUser()
    {
        Console.WriteLine();
        Console.WriteLine("Please input the absolute path to your ontology turtle file (.ttl): ");
        var ontologyPath = GetStringFromUser(
            s => !File.Exists(s),
            "Could not find the ontology on the given path, please try again");

        return ontologyPath;
    }

    private static string GetGraphStoragePathFromUser()
    {
        Console.WriteLine();
        Console.WriteLine("Please input the absolute path to you GraphStorage (.sqlite): ");
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

    private static string GetStringFromUser(Func<string, bool> errorPredicate, string errorMessage)
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

    private static int GetIntFromUser(Func<int, bool> errorPredicate, string errorMessage)
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

    private static bool GetBoolFromUser(string trueEquivalent, string falseEquivalent)
    {
        var boolString = GetStringFromUser(
            s =>
                !string.Equals(s, trueEquivalent, StringComparison.CurrentCultureIgnoreCase)
                &&
                !string.Equals(s, falseEquivalent, StringComparison.CurrentCultureIgnoreCase),
            "Input cannot be converted be parsed, please try again");

        return boolString == trueEquivalent;
    }

    private static int PresentManagedDataStoresResultingInChoice(List<(string, string)> managedDataStores)
    {
        Console.WriteLine();
        Console.WriteLine("These are the currently managed datastores: ");

        var index = 1;

        managedDataStores.ForEach(tuple => Console.WriteLine($"({index++}) : {tuple.Item1} - {tuple.Item2}"));
        Console.WriteLine("\n(0) - Insert new datastore");

        Console.WriteLine();
        Console.WriteLine("Please select which datastore you want to manipulate: ");
        var choice = GetIntFromUser(
            i => i < 0 || i > managedDataStores.Count,
            $"Please choose a number between 0 and {managedDataStores.Count} (inclusive)");

        return choice;
    }

    private static bool PresentExit(string exitFrom)
    {
        Console.WriteLine();
        Console.WriteLine($"Do you want to exit from {exitFrom}? (y/n)");
        return GetBoolFromUser("y", "n");
    }

    private static void InitOrManageDataStore(int choice, List<(string, string)> managedDataStores,
        GraphStorage graphStorage)
    {
        if (choice == 0)
        {
            InitDatastore(graphStorage);
        }
        else
        {
            var chosenTuple = managedDataStores[choice - 1];
            var datastoreUri = UriFactory.Create(chosenTuple.Item1);
            var datastoreType = GraphDataType.GetTypeFromString(chosenTuple.Item2);

            switch (datastoreType)
            {
                case { } when datastoreType == typeof(Sqlite):
                    ManageSelectedDatastore<Sqlite>(datastoreUri, graphStorage);
                    break;
                // case { } when datastoreType == typeof(PostgreSql):
                //     ManageSelectedDatastore<PostgreSql>(datastoreUri, graphStorage);
                //     break;
                default:
                    throw new InteractiveModeException("The type of datastore is not supported: " + datastoreType);
            }
        }
    }

    private static void InitDatastore(GraphStorage graphStorage)
    {
        var baseUri = GetBaseUriFromUser();

        Console.WriteLine();
        Console.WriteLine("Please provide the type of datastore you want to manage: ");
        PresentDataStoreTypes();
        var choice = GetIntFromUser(
            s => s != 0,
            "Please choose a supported datastore");

        switch (choice)
        {
            case 0:
                InitSqlite(graphStorage, baseUri);
                break;
            default:
                throw new InteractiveModeException("The chosen type of datastore is not supported");
        }
    }

    private static void InitSqlite(GraphStorage graphStorage, string baseUri)
    {
        Console.WriteLine();
        Console.WriteLine("Please provide the path to the SQLite: ");
        var managedDataStores = graphStorage.GetListOfManagedDataStoresWithType();

        var sqlitePath = GetStringFromUser(s => false, "");

        var sqlite = new Sqlite("", baseUri, new SQLiteConnection($"Data Source={sqlitePath}"));
        sqlite.BuildFromDataSource();
        graphStorage.InitInsert(sqlite);
    }

    private static void PresentDataStoreTypes()
    {
        Console.WriteLine("(0) : SQLite");
    }

    private static void ManageSelectedDatastore<T>(Uri datastoreUri, GraphStorage graphStorage) where T : DataStore
    {
        var graph = graphStorage.GetLatest(datastoreUri);

        var graphManipulator = new GraphManipulator<T>(graph);

        var stop = false;

        while (!stop)
        {
            Console.WriteLine("Please write manipulation query: ");
            var manipulationQuery = GetStringFromUser(
                s => !graphManipulator.IsValidManipulationQuery(s),
                "The manipulation query is invalid, please try again");

            try
            {
                graphManipulator.ApplyManipulationQuery(manipulationQuery);
            }
            catch (GraphManipulatorException e)
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

            graphStorage.Insert(datastoreUri, graphManipulator.Graph, graphManipulator.Changes);
            if (PresentExit($"manipulating {datastoreUri}"))
            {
                stop = true;
            }
        }
    }
}

public class InteractiveModeException : Exception
{
    public InteractiveModeException(string message) : base(message)
    {
    }
}