using System.CommandLine;

namespace GraphManipulation.Commands;

public static class OptionBuilder
{
    public static Option<T> CreateOption<T>(string name)
    {
        return new Option<T>(name);
    }

    public static Option<T> WithAlias<T>(this Option<T> option, string alias)
    {
        option.AddAlias(alias);
        return option;
    }

    public static Option<T> WithGetDefaultValue<T>(this Option<T> option, Func<object?> getDefaultValue)
    {
        option.SetDefaultValueFactory(getDefaultValue);
        return option;
    }
    
    public static Option<T> WithArity<T>(this Option<T> option, ArgumentArity arity)
    {
        option.Arity = arity;
        return option;
    }

    public static Option<T> WithAllowMultipleArguments<T>(this Option<T> option, bool value)
    {
        option.AllowMultipleArgumentsPerToken = value;
        return option;
    }
    
    public static Option<T> WithDescription<T>(this Option<T> option, string description)
    {
        option.Description = description;
        return option;
    }
}