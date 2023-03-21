using System.Text.RegularExpressions;
using GraphManipulation.Components;
using GraphManipulation.Helpers;
using GraphManipulation.Models.Stores;
using J2N.Collections.Generic.Extensions;

namespace GraphManipulation.Manipulation;

public static class FunctionParser
{
    // The below regular expression takes a command of the structure Command(Uri,Uri)
    // It verifies whether the structure of the command is correct, that the command itself only contain letters
    // And the URI's are syntactically valid.
    private static string ValidManipulationQueryPattern => "^(\\w+)\\(([\\w:#\\/.]+),\\s?([\\w:#\\/.]+)\\)$";

    public static bool IsValidManipulationQuery(string query)
    {
        return Regex.IsMatch(query, ValidManipulationQueryPattern);
    }

    public static void CommandParser<T>(string manipulationQuery, Manipulator<T> graphManipulator,
        MetadataManager metadataManager) where T : Database
    {
        List<string> parameters = GetParametersFromQuery(manipulationQuery);

        string command = GetParametersFromQuery(manipulationQuery)[0];
        Action<Uri, Uri> action;
        
        Match match;
        switch (command)
        {
            case "RENAME":
                match = Regex.Match(manipulationQuery, ValidManipulationQueryPattern);
                action = graphManipulator.Rename;
                action(new Uri(match.Groups[2].ToString()), new Uri(match.Groups[3].ToString()));
                break;
            case "MOVE":
                match = Regex.Match(manipulationQuery, ValidManipulationQueryPattern);
                action = graphManipulator.Move;
                action(UriFromMatchGroups(match, 2), UriFromMatchGroups(match, 3));
                break;
            case "MARK":
                if (IsValidMarkAsPersonalDataQuery(parameters))
                {
                    metadataManager.MarkAsPersonalData(parameters[1], parameters[2]);
                }
                else throw new ManipulatorException("Command not supported");

                break;
            default:
                throw new ManipulatorException("Command not supported");
        }
    }

    public static List<string> GetParametersFromQuery(string query)
    {
        // Splits input string on:
        // " "; ")"; "("; ","
        return Regex.Split(query, " |\\(|\\)|,").ToList();
    }

    private static Uri UriFromParameters(List<string> parameters, int index)
    {
        return new Uri(parameters[index]);
    }

    public static bool IsValidMarkAsPersonalDataQuery(List<string> parameters)
    {
        if (parameters.Count >= 4)
        {
            return parameters[2] == "AS" && parameters[3] == "PERSONAL" &&
                   parameters[4] == "DATA";
        }

        return false;
    }

    static Uri UriFromMatchGroups(Match match, int inputInteger)
    {
        return new Uri(match.Groups[inputInteger].ToString());
    }
}