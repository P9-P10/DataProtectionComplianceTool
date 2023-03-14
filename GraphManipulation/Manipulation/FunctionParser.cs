using System.Text.RegularExpressions;
using GraphManipulation.Components;
using GraphManipulation.Models.Stores;
using J2N.Collections.Generic.Extensions;

namespace GraphManipulation.Manipulation;

public static class FunctionParser
{
    private static string ValidManipulationQueryPattern => "^(\\w+)\\(([\\w:#\\/.]+),\\s?([\\w:#\\/.]+)\\)$";

    public static bool IsValidManipulationQuery(string query)
    {
        return Regex.IsMatch(query, ValidManipulationQueryPattern);
    }

    public static void CommandParser<T>(string manipulationQuery, Manipulator<T> graphManipulator) where T : Database
    {
        var match = Regex.Match(manipulationQuery, ValidManipulationQueryPattern);

        var command = match.Groups[1].ToString().ToUpper();
        var firstUri = new Uri(match.Groups[2].ToString());
        var secondUri = new Uri(match.Groups[3].ToString());

        Action<Uri, Uri> action = command switch
        {
            "MOVE" => graphManipulator.Move,
            "RENAME" => graphManipulator.Rename,
            _ => throw new ManipulatorException("Command not supported")
        };

        action(firstUri, secondUri);
    }
}