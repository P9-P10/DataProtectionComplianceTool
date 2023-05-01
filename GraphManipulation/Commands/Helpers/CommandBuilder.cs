using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

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

    public static Command WithOption<T>(this Command command, out Option<T> outputOption, Option<T> inputOption)
    {
        command.AddOption(inputOption);
        outputOption = inputOption;
        return command;
    }

    public static Command WithArgument<T>(this Command command, out Argument<T> outputArgument,
        Argument<T> inputArgument)
    {
        command.AddArgument(inputArgument);
        outputArgument = inputArgument;
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
        return BuildCommandWithNameAliasSubject(CommandNamer.AddCommand, CommandNamer.AddAlias, subject);
    }

    public static Command BuildUpdateCommand(string subject = "")
    {
        return BuildCommandWithNameAliasSubject(CommandNamer.UpdateCommand, CommandNamer.UpdateAlias, subject);
    }

    public static Command BuildDeleteCommand(string subject = "")
    {
        return BuildCommandWithNameAliasSubject(CommandNamer.DeleteCommand, CommandNamer.DeleteAlias, subject);
    }

    public static Command BuildRemoveCommand(string subject = "")
    {
        return BuildCommandWithNameAliasSubject(CommandNamer.RemoveCommand, CommandNamer.RemoveAlias, subject);
    }

    public static Command BuildListCommand(string subject = "")
    {
        return BuildCommandWithNameAliasSubject(CommandNamer.ListCommand, CommandNamer.ListAlias, subject);
    }

    public static Command BuildSetCommand(string subject = "")
    {
        return BuildCommandWithNameAliasSubject(CommandNamer.SetCommand, CommandNamer.SetAlias, subject);
    }

    public static Command BuildShowCommand(string subject = "")
    {
        return BuildCommandWithNameAliasSubject(CommandNamer.ShowCommand, CommandNamer.ShowAlias,subject);
    }
}

public class CommandException : Exception
{
    public CommandException(string message) : base(message)
    {
    }
}