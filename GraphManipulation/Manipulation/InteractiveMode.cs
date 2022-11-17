using VDS.RDF;
using VDS.RDF.Parsing;
using GraphManipulation.Extensions;

namespace GraphManipulation.Manipulation;

public class InteractiveMode
{
    public InteractiveMode()
    {
        
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
        }
        while (!flag);

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
    
    public void Run()
    {
        Console.WriteLine();
        Console.WriteLine("Welcome to this interactive experience!");
        
        Console.WriteLine();
        Console.WriteLine("Please input the absolute path to your ontology turtle file (.ttl): ");
        var ontologyPath = GetStringFromUser(
            s => !File.Exists(s),
            "Could not find the ontology on the given path, please try again");

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
        
        IGraph ontology = new Graph();
        ontology.LoadFromFile(ontologyPath, new TurtleParser());

        var graphStorage = new GraphStorage(graphStoragePath, ontology);
        
        Console.WriteLine();
        Console.WriteLine("These are the currently managed datastores: ");

        var index = 1;
        
        var managedDataStores = graphStorage.GetListOfManagedDataStoresWithType();
        
        managedDataStores.ForEach(tuple => Console.WriteLine($"({index++}) : {tuple.Item1} - {tuple.Item2}"));
        Console.WriteLine("\n(0) - Insert new datastore");
        
        Console.WriteLine();
        Console.WriteLine("Please select which datastore you want to manipulate: ");
        var choice = GetIntFromUser(
            i => i < 0 || i > managedDataStores.Count,
            $"Please choose a number between 0 and {managedDataStores.Count} (inclusive)");
        
        if (choice == 0)
        {
            
        }
        else
        {
            var chosenTuple = managedDataStores[choice - 1];
            var datastoreUri = UriFactory.Create(chosenTuple.Item1);
            var datastoreType = GraphDataType.GetTypeFromString(chosenTuple.Item2);
            
            ManageSelectedDatastore(datastoreUri, datastoreType);
        }
    }

    public void ManageSelectedDatastore(Uri datastoreUri, Type datastoreType)
    {
        
    }
}