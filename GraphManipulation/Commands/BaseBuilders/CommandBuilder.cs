using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

namespace GraphManipulation.Commands.BaseBuilders;

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

    public static Command WithSubCommand(this Command command, Command subCommand)
    {
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
}