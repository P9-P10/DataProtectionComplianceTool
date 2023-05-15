using System.CommandLine;
using GraphManipulation.Commands.Builders.Binders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Helpers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Base;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Commands.Builders;

public abstract class BaseCommandBuilder
{
    public abstract Command Build();
}

public abstract class BaseCommandBuilder<TKey, TValue> : BaseCommandBuilder
    where TKey : notnull
    where TValue : Entity<TKey>, IListable, new()
{
    protected readonly Handler<TKey, TValue> Handler;
    protected readonly FeedbackEmitter<TKey, TValue> Emitter;

    protected BaseCommandBuilder(IHandlerFactory handlerFactory)
    {
        Emitter = new FeedbackEmitter<TKey, TValue>();
        Handler = handlerFactory.CreateHandler<TKey, TValue>(Emitter, StatusReport);
    }

    protected Command Build(string name, string alias, out Option<TKey> keyOption)
    {
        keyOption = BuildKeyOption();

        return CommandBuilder.CreateNewCommand(name)
            .WithAlias(alias)
            .WithSubCommands(
                DeleteCommand(keyOption),
                ListCommand(),
                ShowCommand(keyOption),
                StatusCommand()
            );
    }

    protected Command CreateCommand(Option<TKey> keyOption, BaseBinder<TKey, TValue> binder,
        Option[] options)
    {
        var command = CommandBuilder
            .BuildCreateCommand()
            .WithDescription($"Creates a new {GetEntityType()} in the system")
            .WithOption(out _, keyOption)
            .WithOptions(options);

        command.SetHandler(Handler.CreateHandler, keyOption, binder);

        return command;
    }

    protected Command UpdateCommand(Option<TKey> keyOption, BaseBinder<TKey, TValue> binder,
        Option[] options)
    {
        var command = CommandBuilder
            .BuildUpdateCommand()
            .WithDescription($"Updates the given {GetEntityType()} with the given values")
            .WithOption(out _, keyOption)
            .WithOptions(options);

        command.SetHandler(Handler.UpdateHandler, keyOption, binder);

        return command;
    }

    private Command DeleteCommand(Option<TKey> keyOption)
    {
        var command = CommandBuilder
            .BuildDeleteCommand()
            .WithDescription($"Deletes the given {GetEntityType()} from the system")
            .WithOption(out _, keyOption);

        command.SetHandler(Handler.DeleteHandler, keyOption);

        return command;
    }

    protected Command ShowCommand(Option<TKey> keyOption)
    {
        var command = CommandBuilder
            .BuildShowCommand()
            .WithDescription($"Shows details about the given {GetEntityType()}")
            .WithOption(out _, keyOption);

        command.SetHandler(Handler.ShowHandler, keyOption);

        return command;
    }

    protected Command ListCommand()
    {
        var command = CommandBuilder
            .BuildListCommand()
            .WithDescription($"Lists the {GetEntityType()}(e)s currently in the system");

        command.SetHandler(Handler.ListHandler);
        return command;
    }

    protected Command StatusCommand()
    {
        var command = CommandBuilder
            .BuildStatusCommand()
            .WithDescription($"Shows the status(es) of the {GetEntityType()}(e)s currently in the system");

        command.SetHandler(() => Handler.StatusHandler());
        return command;
    }

    protected Command ListChangesCommand<TK, TV>(Option<TKey> keyOption, Option<IEnumerable<TK>> listOption,
        IManager<TK, TV> manager, bool isAdd, Func<TValue, IEnumerable<TV>> getCurrentList,
        Action<TValue, IEnumerable<TV>> setList)
        where TV : Entity<TK>
    {
        var otherValueType = TypeToString.GetEntityType(typeof(TV)).Replace(" ", "-");

        var command = (isAdd
                ? CommandBuilder.BuildAddCommand(otherValueType)
                : CommandBuilder.BuildRemoveCommand(otherValueType))
            .WithDescription(
                $"{(isAdd ? "Adds" : "Removes")} the given {otherValueType}(e)s {(isAdd ? "to" : "from")} the {GetEntityType()}")
            .WithOption(out _, keyOption)
            .WithOption(out _, listOption);

        command.SetHandler((key, list) => { Handler.ListChangesHandler(key, list, getCurrentList, setList, isAdd, manager); },
            keyOption, listOption);

        return command;
    }

    protected (Command Add, Command Remove) BuildListChangesCommand<TK, TV>(Option<TKey> keyOption,
        Option<IEnumerable<TK>> listOption,
        IManager<TK, TV> manager, Func<TValue, IEnumerable<TV>> getCurrentList,
        Action<TValue, IEnumerable<TV>> setList)
        where TV : Entity<TK>
    {
        var addCommand = ListChangesCommand(keyOption, listOption, manager, true, getCurrentList, setList);
        var removeCommand = ListChangesCommand(keyOption, listOption, manager, false, getCurrentList, setList);

        return (addCommand, removeCommand);
    }

    protected abstract Option<TKey> BuildKeyOption();
    
    protected abstract void StatusReport(TValue value);

    private static string GetEntityType()
    {
        return TypeToString.GetEntityType(typeof(TValue));
    }
}