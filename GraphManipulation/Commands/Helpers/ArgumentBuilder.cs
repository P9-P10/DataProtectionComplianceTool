using System.CommandLine;

namespace GraphManipulation.Commands.Helpers;

public static class ArgumentBuilder
{
    public static Argument<T> CreateArgument<T>(string name)
    {
        return new Argument<T>(name);
    }

    public static Argument<T> WithDescription<T>(this Argument<T> argument, string description)
    {
        argument.Description = description;
        return argument;
    }

    public static Argument<string> BuildStringArgument(string name = "value")
    {
        return new Argument<string>(name);
    }

    public static Argument<int> BuildIntArgument(string name = "value")
    {
        return new Argument<int>(name);
    }
}