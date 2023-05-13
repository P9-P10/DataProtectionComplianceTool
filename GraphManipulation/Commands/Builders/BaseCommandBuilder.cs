using System.CommandLine;
using System.CommandLine.IO;
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
    protected readonly IConsole Console;

    protected BaseCommandBuilder(IConsole console, IManager<TKey, TValue> manager)
    {
        Console = console;
        Manager = manager;
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
    
    private void CreateHandler(TKey key, TValue value)
    {
        if (Manager.Get(key) is not null)
        {
            // Cannot create something that exists already
            EmitAlreadyExists(key);
            return;
        }

        if (Manager.Create(key))
        {
            EmitSuccess(key, Operations.Created);
            UpdateHandler(key, value);
        }
        else
        {
            // Could not create entity
            EmitFailure(key, Operations.Created);
        }
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
    
    private void UpdateHandler(TKey key, TValue value)
    {
        var old = Manager.Get(key);

        if (old is null)
        {
            // Can only update something that exists
            EmitCouldNotFind(key);
            return;
        }

        if (!key.Equals(value.Key!) && Manager.Get(value.Key!) is not null)
        {
            // If the key is updated, it can only be updated to something that doesn't already exist
            EmitAlreadyExists(value.Key!);
            return;
        }

        old.Fill(value);

        if (Manager.Update(key, value))
        {
            EmitSuccess(key, Operations.Updated, value);
            StatusReport(Manager.Get(key)!);
        }
        else
        {
            EmitFailure(key, Operations.Updated, value);
        }
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
    
    private void DeleteHandler(TKey key)
    {
        if (Manager.Get(key) is null)
        {
            // Can only delete something that exists
            EmitCouldNotFind(key);
            return;
        }

        if (Manager.Delete(key))
        {
            EmitSuccess(key, Operations.Deleted);
        }
        else
        {
            EmitFailure(key, Operations.Deleted);
        }
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
    
    private void ShowHandler(TKey key)
    {
        if (Manager.Get(key) is null)
        {
            // Can only show something that exists
            EmitCouldNotFind(key);
            return;
        }

        Console.WriteLine(Manager.Get(key)!.ToListing());
    }
    
    protected Command ListCommand()
    {
        var command = CommandBuilder
            .BuildListCommand()
            .WithDescription($"Lists the {GetEntityType()}(e)s currently in the system");

        command.SetHandler(ListHandler);
        return command;
    }
    
    private void ListHandler()
    {
        Manager.GetAll().Select(r => r.ToListing()).ToList().ForEach(Console.WriteLine);
    }

    protected Command ListChangesCommand<TK, TV>(Option<TKey> keyOption, Option<IEnumerable<TK>> listOption,
        IManager<TK, TV> manager, bool isAdd, Func<TValue, IEnumerable<TV>> getCurrentList,
        Action<TValue, IEnumerable<TV>> setList)
    {
        var otherValueType = TypeToString.GetEntityType(typeof(TV));
        
        var command = (isAdd 
                ? CommandBuilder.BuildAddCommand(otherValueType) 
                : CommandBuilder.BuildRemoveCommand(otherValueType)) 
            .WithDescription($"{(isAdd ? "Adds" : "Removes")} the given {otherValueType}(e)s {(isAdd ? "to" : "from")} the {GetEntityType()}")
            .WithOption(out _, keyOption)
            .WithOption(out _, listOption);

        command.SetHandler((key, list) =>
        {
            ListChangesHandler(key, list, getCurrentList, setList, isAdd, manager);
        }, keyOption, listOption);
        
        return command;
    }

    protected (Command Add, Command Remove) BuildListChangesCommand<TK, TV>(Option<TKey> keyOption,
        Option<IEnumerable<TK>> listOption,
        IManager<TK, TV> manager, Func<TValue, IEnumerable<TV>> getCurrentList,
        Action<TValue, IEnumerable<TV>> setList)
    {
        var addCommand = ListChangesCommand(keyOption, listOption, manager, true, getCurrentList, setList);
        var removeCommand = ListChangesCommand(keyOption, listOption, manager, false, getCurrentList, setList);

        return (addCommand, removeCommand);
    }

    private void ListChangesHandler<TK, TV>(
        TKey key,
        IEnumerable<TK> list,
        Func<TValue, IEnumerable<TV>> getCurrentList,
        Action<TValue, IEnumerable<TV>> setList,
        bool isAdd,
        IGetter<TV, TK> manager)
    {
        var value = Manager.Get(key);
        if (value is null)
        {
            EmitCouldNotFind(key);
            return;
        }

        var currentList = getCurrentList(value).ToList();
        
        foreach (var k in list)
        {
            var v = manager.Get(k);

            if (v is null)
            {
                EmitCouldNotFind<TK, TV>(k);
                return;
            }

            if (isAdd)
            {
                if (!currentList.Contains(v))
                {
                    currentList.Add(v);
                }
            }
            else
            {
                if (currentList.Contains(v))
                {
                    currentList.Remove(v);
                }
            }
        }

        setList(value, currentList);
        UpdateHandler(key, value);
    }

    protected abstract void StatusReport(TValue value);

    protected Command StatusCommand()
    {
        var command = CommandBuilder
            .BuildStatusCommand()
            .WithDescription($"Show the status(es) of the {GetEntityType()}(e)s currently in the system");

        command.SetHandler(() => StatusHandler(StatusReport));
        return command;
    }

    private void StatusHandler(Action<TValue> statusAction)
    {
        Manager.GetAll().ToList().ForEach(statusAction);
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

    protected void EmitSuccess(TKey key, Operations operation, TValue? value = null)
    {
        Console.WriteLine(SuccessMessage(key, operation, value));
    }

    protected void EmitFailure(TKey key, Operations operation, TValue? value = null)
    {
        Console.Error.WriteLine(FailureMessage(key, operation, value));
    }

    protected void EmitAlreadyExists(TKey key)
    {
        Console.Error.WriteLine(AlreadyExistsMessage(key));
    }

    protected void EmitCouldNotFind(TKey key)
    {
        EmitCouldNotFind<TKey, TValue>(key);
    }

    protected void EmitCouldNotFind<TK, TV>(TK key)
    {
        Console.Error.WriteLine(CouldNotFindMessage<TK, TV>(key));
    }

    protected void EmitMissing<TV>(TKey subject)
    {
        Console.WriteLine(MissingMessage(subject, TypeToString.GetEntityType(typeof(TV))));
    }

    protected void EmitMissing(TKey subject, string obj)
    {
        Console.WriteLine(MissingMessage(subject, obj));
    }

    private static string MissingMessage(TKey subject, string obj)
    {
        return $"{subject} {GetEntityType()} is missing a(n) {obj}";
    }

    private static string CouldNotFindMessage<TK, TV>(TK key)
    {
        return $"Could not find {TypeToString.GetEntityType(typeof(TV))} using {key}";
    }

    private static string AlreadyExistsMessage(TKey key)
    {
        return AlreadyExistsMessage(key, typeof(TValue));
    }

    private static string AlreadyExistsMessage(TKey key, Type type)
    {
        return $"Found an existing {TypeToString.GetEntityType(type)} using {key}";
    }

    protected static string SuccessMessage(TKey key, Operations operation, TValue? value)
    {
        return $"Successfully {OperationToString(operation)} {key} {GetEntityType()}" +
               (value is not null ? $" with {value.ToListing()}" : "");
    }

    protected static string FailureMessage(TKey key, Operations operation, TValue? value)
    {
        return $"{key} {GetEntityType()} could not be {OperationToString(operation)}" +
               (value is not null ? $" with {value.ToListing()}" : "");
    }

    protected static string OperationToString(Operations operation)
    {
        return operation.ToString().ToLower();
    }

    private static string GetEntityType()
    {
        return TypeToString.GetEntityType(typeof(TValue));
    }
    
    protected enum Operations
    {
        Updated,
        Deleted,
        Created,
        Set,
        Removed,
        Executed
    }
}