using System.CommandLine;
using GraphManipulation.Commands.Builders.Binders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Helpers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Base;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Commands.Builders;

public abstract class BaseCommandBuilder<TKey, TValue>
    where TKey : notnull
    where TValue : Entity<TKey>, IListable, new()
{
    protected readonly IManager<TKey, TValue> Manager;
    protected readonly FeedbackEmitter<TKey, TValue> Emitter;

    protected BaseCommandBuilder(IManager<TKey, TValue> manager)
    {
        Manager = manager;
        Emitter = new FeedbackEmitter<TKey, TValue>();
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

    public abstract Command Build();

    protected Command CreateCommand(Option<TKey> keyOption, BaseBinder<TKey, TValue> binder,
        Option[] options)
    {
        var command = CommandBuilder
            .BuildCreateCommand()
            .WithDescription($"Creates a new {GetEntityType()} in the system")
            .WithOption(out _, keyOption)
            .WithOptions(options);

        command.SetHandler(CreateHandler, keyOption, binder);

        return command;
    }

    protected Command UpdateCommand(Option<TKey> keyOption, BaseBinder<TKey, TValue> binder,
        Option[] options)
    {
        var command = CommandBuilder
            .BuildUpdateCommand()
            .WithDescription($"Creates the given {GetEntityType()} with the given values")
            .WithOption(out _, keyOption)
            .WithOptions(options);

        command.SetHandler(UpdateHandler, keyOption, binder);

        return command;
    }

    private Command DeleteCommand(Option<TKey> keyOption)
    {
        var command = CommandBuilder
            .BuildDeleteCommand()
            .WithDescription($"Deletes the given {GetEntityType()} from the system")
            .WithOption(out _, keyOption);

        command.SetHandler(DeleteHandler, keyOption);

        return command;
    }

    protected Command ShowCommand(Option<TKey> keyOption)
    {
        var command = CommandBuilder
            .BuildShowCommand()
            .WithDescription($"Shows details about the given {GetEntityType()}")
            .WithOption(out _, keyOption);

        command.SetHandler(ShowHandler, keyOption);

        return command;
    }

    protected Command ListCommand()
    {
        var command = CommandBuilder
            .BuildListCommand()
            .WithDescription($"Lists the {GetEntityType()}(e)s currently in the system");

        command.SetHandler(ListHandler);
        return command;
    }

    protected Command StatusCommand()
    {
        var command = CommandBuilder
            .BuildStatusCommand()
            .WithDescription($"Show the status(es) of the {GetEntityType()}(e)s currently in the system");

        command.SetHandler(() => StatusHandler(StatusReport));
        return command;
    }

    protected Command ListChangesCommand<TK, TV>(Option<TKey> keyOption, Option<IEnumerable<TK>> listOption,
        IManager<TK, TV> manager, bool isAdd, Func<TValue, IEnumerable<TV>> getCurrentList,
        Action<TValue, IEnumerable<TV>> setList)
        where TV : Entity<TK>
    {
        var otherValueType = TypeToString.GetEntityType(typeof(TV));

        var command = (isAdd
                ? CommandBuilder.BuildAddCommand(otherValueType)
                : CommandBuilder.BuildRemoveCommand(otherValueType))
            .WithDescription(
                $"{(isAdd ? "Adds" : "Removes")} the given {otherValueType}(e)s {(isAdd ? "to" : "from")} the {GetEntityType()}")
            .WithOption(out _, keyOption)
            .WithOption(out _, listOption);

        command.SetHandler((key, list) => { ListChangesHandler(key, list, getCurrentList, setList, isAdd, manager); },
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

    private void CreateHandler(TKey key, TValue value)
    {
        Handlers<TKey, TValue>.CreateHandler(key, value, Manager, Emitter, StatusReport);
    }
    
    private void UpdateHandler(TKey key, TValue value)
    {
        Handlers<TKey, TValue>.UpdateHandler(key, value, Manager, Emitter, StatusReport);
    }
    
    private void DeleteHandler(TKey key)
    {
        Handlers<TKey, TValue>.DeleteHandler(key, Manager, Emitter);
    }
    
    private void ShowHandler(TKey key)
    {
        Handlers<TKey, TValue>.ShowHandler(key, Manager, Emitter);
    }
    
    private void ListHandler()
    {
        Handlers<TKey, TValue>.ListHandler(Manager);
    }

    private void StatusHandler(Action<TValue> statusAction)
    {
        Handlers<TKey, TValue>.StatusHandler(statusAction, Manager);
    }
    
    private void ListChangesHandler<TK, TV>(
        TKey key,
        IEnumerable<TK> list,
        Func<TValue, IEnumerable<TV>> getCurrentList,
        Action<TValue, IEnumerable<TV>> setList,
        bool isAdd,
        IGetter<TV, TK> manager)
        where TV : Entity<TK>
    {
        Handlers<TKey, TValue>.ListChangesHandler(key, list, getCurrentList, setList, isAdd, Manager, manager, Emitter,
            new FeedbackEmitter<TK, TV>(), StatusReport);
    }

    protected Option<TKey> BuildKeyOption(string name, string alias, string description)
    {
        return OptionBuilder
            .CreateOption<TKey>(name)
            .WithAlias(alias)
            .WithDescription(description)
            .WithIsRequired(true);
    }

    protected abstract Option<TKey> BuildKeyOption();
    
    protected abstract void StatusReport(TValue value);

    protected Option<string> BuildDescriptionOption()
    {
        return OptionBuilder
            .CreateDescriptionOption()
            .WithDescription($"The description of the {GetEntityType()}");
    }

    protected Option<string> BuildNewNameOption()
    {
        return OptionBuilder
            .CreateNewNameOption()
            .WithDescription($"The new name of the {GetEntityType()}");
    }
    
    private static string GetEntityType()
    {
        return TypeToString.GetEntityType(typeof(TValue));
    }
}