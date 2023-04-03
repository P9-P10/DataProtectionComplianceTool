namespace GraphManipulation.Helpers;

public static class UserInteraction
{
    public static string GetOntologyPathFromUser()
    {
        Console.WriteLine();
        Console.WriteLine("Please input the absolute path to your ontology turtle file (.ttl): ");
        var ontologyPath = GetStringFromUser(
            s => !File.Exists(s),
            "Could not find the ontology on the given path, please try again");

        return ontologyPath;
    }

    public static string GetGraphStorageConnectionStringFromUser()
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

    public static string GetStringFromUser(Func<string, bool> errorPredicate, string errorMessage)
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

    public static int GetIntFromUser(Func<int, bool> errorPredicate, string errorMessage)
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

    public static bool GetBoolFromUser(string trueEquivalent, string falseEquivalent)
    {
        var boolString = GetStringFromUser(
            s =>
                !string.Equals(s, trueEquivalent, StringComparison.CurrentCultureIgnoreCase)
                &&
                !string.Equals(s, falseEquivalent, StringComparison.CurrentCultureIgnoreCase),
            "Input cannot be converted be parsed, please try again");

        return boolString == trueEquivalent;
    }

    public static int PresentManagedDatabasesReturnChoice(List<(string, string)> managedDatabases)
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

    public static bool PresentExit(string exitFrom)
    {
        Console.WriteLine();
        Console.WriteLine($"Do you want to exit from {exitFrom}? (y/n)");
        return GetBoolFromUser("y", "n");
    }
}