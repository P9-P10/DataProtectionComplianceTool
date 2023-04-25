using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Commands.Helpers;

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

    public static Command WithOptions(this Command command, params Option[] options)
    {
        foreach (var option in options)
            command.AddOption(option);

        return command;
    }

    public static Command WithOption<T>(this Command command, out Option<T> outputOption, Option<T> inputOption)
    {
        command.AddOption(inputOption);
        outputOption = inputOption;
        return command;
    }

    public static Command WithArguments(this Command command, params Argument[] arguments)
    {
        foreach (var argument in arguments)
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

    public static Command WithHandler(this Command command, Action handle)
    {
        command.SetHandler(handle);
        return command;
    }

    public static Command WithHandler(this Command command, ICommandHandler handler)
    {
        command.Handler = handler;
        return command;
    }

    public static Command WithDescription(this Command command, string description)
    {
        command.Description = description;
        return command;
    }

    public static Command WithSubCommands(this Command command, params Command[] subCommands)
    {
        foreach (var subCommand in subCommands)
            command.AddCommand(subCommand);

        return command;
    }

    private static Command BuildCommandWithNameAliasSubject(string name, string alias, string subject = "")
    {
        return CreateCommand(name + (string.IsNullOrEmpty(subject) ? "" : $"-{subject}"))
            .WithAlias(alias + (string.IsNullOrEmpty(subject) ? "" : $"{subject.First()}"));
    }

    public static Command BuildAddCommand(string subject = "")
    {
        return BuildCommandWithNameAliasSubject("add", "a", subject);
    }

    public static Command BuildUpdateCommand(string subject = "")
    {
        return BuildCommandWithNameAliasSubject("update", "u", subject);
    }

    public static Command BuildDeleteCommand(string subject = "")
    {
        return BuildCommandWithNameAliasSubject("delete", "d", subject);
    }

    public static Command BuildRemoveCommand(string subject = "")
    {
        return BuildCommandWithNameAliasSubject("remove", "r", subject);
    }

    public static Command BuildListCommand(string subject = "")
    {
        return BuildCommandWithNameAliasSubject("list", "ls", subject);
    }

    public static Command BuildSetCommand(string subject = "")
    {
        return BuildCommandWithNameAliasSubject("set", "st", subject);
    }

    public static Command BuildShowCommand(string subject = "")
    {
        return BuildCommandWithNameAliasSubject("show", "sh", subject);
    }

    public static Command BuildListCommand<TResult, TKey>(IConsole console, IGetter<TResult, TKey> getter,
        string subject = "")
        where TResult : IListable
    {
        return BuildListCommand(subject)
            .WithHandler(() => getter
                .GetAll()
                .ToList()
                .ForEach(r => console.WriteLine(r.ToListing())
                ));
    }

    public static Command BuildShowCommand<TResult, TKey>(IConsole console, IGetter<TResult, TKey> getter,
        Option<TKey> keyOption, string failureSubject,
        string subject = "")
        where TResult : IListable
    {
        var command = BuildShowCommand(subject);

        command.SetHandler(key =>
        {
            var value = getter.Get(key);

            console.WriteLine(value != null ? value.ToListing() : BuildFailureToFindMessage(failureSubject, key));
        }, keyOption);

        return command;
    }

    public static Command BuildDeleteCommand<TManager, TResult, TKey>(IConsole console, TManager manager,
        Option<TKey> keyOption, string failureSubject, string subject = "")
        where TResult : IListable
        where TManager : IGetter<TResult, TKey>, IDeleter<TKey>
    {
        var command = BuildDeleteCommand(subject);

        command.SetHandler(key =>
        {
            var value = manager.Get(key);

            if (value is null)
            {
                console.WriteLine(BuildFailureToFindMessage(failureSubject, key));
                return;
            }

            manager.Delete(key);
        }, keyOption);

        return command;
    }

    public static string BuildFailureToFindMessage<TKey>(string failureSubject, TKey key)
    {
        return $"Could not find {failureSubject} using \"{key}\"";
    }

    public static string BuildAlreadyExistsMessage<TKey>(string failureSubject, TKey key)
    {
        return $"Found an existing {failureSubject} using \"{key}\", aborting";
    }
}

public class CommandException : Exception
{
    public CommandException(string message) : base(message)
    {
    }
}