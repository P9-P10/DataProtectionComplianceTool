using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Archive;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Helpers;

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

    public static Option<T> WithDefaultValue<T>(this Option<T> option, T defaultValue)
    {
        option.SetDefaultValue(defaultValue);
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

    public static Option<T> WithIsRequired<T>(this Option<T> option, bool value)
    {
        option.IsRequired = value;
        return option;
    }

    public static Option<T> WithDescription<T>(this Option<T> option, string description)
    {
        option.Description = description;
        return option;
    }

    public static Option<int> CreateIdOption()
    {
        return CreateOption<int>(OptionNamer.Id).WithAlias(OptionNamer.IdAlias);
    }

    public static Option<string> CreateDescriptionOption()
    {
        return CreateOption<string>(OptionNamer.Description).WithAlias(OptionNamer.DescriptionAlias);
    }

    public static Option<TableColumnPair> CreateTableColumnPairOption()
    {
        return new Option<TableColumnPair>(
                OptionNamer.TableColumn,
                result =>
                {
                    if (result.Tokens.Count == 2)
                    {
                        return new TableColumnPair(result.Tokens[0].Value, result.Tokens[1].Value);
                    }

                    result.ErrorMessage = $"{OptionNamer.TableColumn} requires two arguments";
                    return new TableColumnPair("", "");
                })
            .WithAlias(OptionNamer.TableColumnAlias)
            .WithIsRequired(true)
            .WithArity(new ArgumentArity(2, 2))
            .WithAllowMultipleArguments(true);
    }

    public static Option<IEnumerable<string>> CreatePurposeListOption()
    {
        return CreateOption<IEnumerable<string>>(OptionNamer.PurposeList)
            .WithAlias(OptionNamer.PurposeListAlias)
            .WithAllowMultipleArguments(true)
            .WithArity(ArgumentArity.OneOrMore);
    }

    public static Option<string> CreateNameOption()
    {
        return CreateOption<string>(OptionNamer.Name).WithAlias(OptionNamer.NameAlias);
    }

    public static Option<string> CreateNewNameOption()
    {
        return CreateOption<string>(OptionNamer.NewName).WithAlias(OptionNamer.NewNameAlias);
    }

    public static void ValidInterval(CommandResult commandResult, Option<string> option)
    {
        if (commandResult.FindResultFor(option) is null)
        {
            return;
        }

        try
        {
            string result = commandResult.GetValueForOption(option);
            if (result == null)
            {
                return;
            }

            if (!VacuumingRule.IsValidInterval(result))
            {
                commandResult.ErrorMessage =
                    "Please provide valid vacuuming interval (Should be of the format 2y 3m 2w)";
            }
        }
        catch (InvalidOperationException e)
        {
            // Ignore here, is dealt with somewhere else
        }
    }

    public static void ValidateOrder<TEnumerable, TValue>(CommandResult commandResult, Option<TEnumerable> option)
        where TEnumerable : IEnumerable<TValue>
    {
        if (commandResult.FindResultFor(option) is null)
        {
            return;
        }

        try
        {
            var enumerable = commandResult.GetValueForOption(option);

            if (enumerable is null)
            {
                return;
            }

            var list = enumerable.ToList();

            if (!list.OrderBy(e => e).SequenceEqual(list))
            {
                commandResult.ErrorMessage = "Minimum was higher than maximum";
            }
        }
        catch (InvalidOperationException)
        {
            // Ignore here, is dealt with somewhere else
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