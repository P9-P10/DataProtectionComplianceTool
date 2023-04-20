using System.CommandLine;

namespace GraphManipulation.Commands;

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

}