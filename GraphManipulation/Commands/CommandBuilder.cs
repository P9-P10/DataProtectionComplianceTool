using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Diagnostics;

namespace GraphManipulation.Commands;

public static class CommandBuilder
{
    public static Command CreateCommand(string name)
    {
        return new Command(name);
    }

    public static Command WithAlias(this Command command, string alias)
    {
        command.AddAlias(alias);
        return command;
    }

    public static Command WithOption(this Command command, Option option)
    {
        command.AddOption(option);
        return command;
    }

    public static Command WithArgument(this Command command, Argument argument)
    {
        command.AddArgument(argument);
        return command;
    }

    public static Command WithValidator(this Command command, ValidateSymbolResult<CommandResult> validate)
    {
        command.AddValidator(validate);
        return command;
    }

    public static Command WithHandler(this Command command, Action<InvocationContext> handle)
    {
        command.SetHandler(handle);
        return command;
    }
    
    public static Command WithDescription(this Command command, string description)
    {
        command.Description = description;
        return command;
    }
    
    public static Option<int> BuildIdOption(string description)
    {
        return BuildOption<int>(
            "--id",
            description,
            "-i"
        );
    }

    public static Option<bool> BuildAllOption(string description)
    {
        return BuildOption<bool>(
            "--all", 
            description, 
            "-a"
        );
    }
    
    public static Argument<string> BuildStringArgument(string name = "value") => new(name);
    public static Argument<int> BuildIntArgument(string name = "value") => new(name);

    public static Option<T> BuildOption<T>(string name, string description, string alias)
    {
        var option = new Option<T>(name: name, description: description);
        option.AddAlias(alias);
        return option;
    }

    public static Option<T> BuildOption<T>(string name, string description, string alias, Func<T> getDefaultValue)
    {
        var option = new Option<T>(name: name, description: description, getDefaultValue: getDefaultValue);
        option.AddAlias(alias);
        return option;
    }

    public static void ValidateOrder<T>(CommandResult commandResult, Option<IEnumerable<T>> option)
    {
        if (commandResult.FindResultFor(option) is null)
        {
            return;
        }

        try
        {
            var list = (commandResult.GetValueForOption(option) ?? Array.Empty<T>()).ToList();

            if (!list.OrderBy(e => e).SequenceEqual(list))
            {
                commandResult.ErrorMessage = "Minimum was higher than maximum";
            }
        }
        catch (InvalidOperationException)
        {
            // Ignore for now
        }
    }

    // https://github.com/dorssel/usbipd-win/blob/2f7cbb732889ed00617e85f2f0b22239e8533960/Usbipd/Program.cs#L86-L94
    public static void ValidateOneOf(CommandResult commandResult, params Option[] options)
    {
        Debug.Assert(options.Length >= 2);

        if (options.Count(option => commandResult.FindResultFor(option) is not null) != 1)
        {
            commandResult.ErrorMessage = OneOfRequiredText(options);
        }
    }

    // https://github.com/dorssel/usbipd-win/blob/2f7cbb732889ed00617e85f2f0b22239e8533960/Usbipd/Program.cs#L86-L94
    public static string OneOfRequiredText(params Option[] options)
    {
        Debug.Assert(options.Length >= 2);

        var names = options.Select(o => $"'--{o.Name}'").ToArray();
        var list = names.Length == 2
            ? $"{names[0]} or {names[1]}"
            : string.Join(", ", names[..^1]) + ", or " + names[^1];
        return $"Exactly one of the options {list} is required.";
    }
}