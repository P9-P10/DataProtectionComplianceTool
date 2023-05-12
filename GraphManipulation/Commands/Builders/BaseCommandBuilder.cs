using System.CommandLine;
using System.CommandLine.IO;
using GraphManipulation.Commands.Builders.Binders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Managers.Interfaces.Base;
using GraphManipulation.Models;
using GraphManipulation.Models.Base;
using GraphManipulation.Models.Interfaces.Base;

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
                ShowCommand(keyOption)
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

    private Command ShowCommand(Option<TKey> keyOption)
    {
        var command = CommandBuilder
            .BuildShowCommand()
            .WithDescription($"Shows details about the given {GetEntityType()}")
            .WithOption(out _, keyOption);

        command.SetHandler(ShowHandler, keyOption);

        return command;
    }
    
    private Command ListCommand()
    {
        var command = CommandBuilder
            .BuildListCommand()
            .WithDescription($"Lists the {GetEntityType()}(e)s currently in the system");

        command.SetHandler(ListHandler);
        return command;
    }

    protected Command ListChangesCommand<TK, TV>(Option<TKey> keyOption, Option<IEnumerable<TK>> listOption,
        IManager<TK, TV> manager, bool isAdd, Func<TValue, IEnumerable<TV>> getCurrentList,
        Action<TValue, IEnumerable<TV>> setList)
    {
        var otherValueType = GetEntityType(typeof(TV));
        
        var command = CommandBuilder
            .BuildAddCommand(otherValueType)
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

    private void CreateHandler(TKey key, TValue value)
    {
        if (Manager.Get(key) is not null)
        {
            // Cannot create something that exists already
            WriteAlreadyExists(key);
            return;
        }

        if (Manager.Create(key))
        {
            WriteSuccess(key, Operations.Created);
            UpdateHandler(key, value);
        }
        else
        {
            // Could not create entity
            WriteFailure(key, Operations.Created);
        }
    }

    private void UpdateHandler(TKey key, TValue value)
    {
        var old = Manager.Get(key);

        if (old is null)
        {
            // Can only update something that exists
            WriteCouldNotFind(key);
            return;
        }

        if (!key.Equals(value.Key!) && Manager.Get(value.Key!) is not null)
        {
            // If the key is updated, it can only be updated to something that doesn't already exist
            WriteAlreadyExists(value.Key!);
            return;
        }

        old.Fill(value);

        if (Manager.Update(key, value))
        {
            WriteSuccess(key, Operations.Updated, value);
        }
        else
        {
            WriteFailure(key, Operations.Updated, value);
        }
    }

    private void DeleteHandler(TKey key)
    {
        if (Manager.Get(key) is null)
        {
            // Can only delete something that exists
            WriteCouldNotFind(key);
            return;
        }

        if (Manager.Delete(key))
        {
            WriteSuccess(key, Operations.Deleted);
        }
        else
        {
            WriteFailure(key, Operations.Deleted);
        }
    }

    private void ShowHandler(TKey key)
    {
        if (Manager.Get(key) is null)
        {
            // Can only show something that exists
            WriteCouldNotFind(key);
            return;
        }

        Console.WriteLine(Manager.Get(key)!.ToListing());
    }

    private void ListHandler()
    {
        Manager.GetAll().Select(r => r.ToListing()).ToList().ForEach(Console.WriteLine);
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
            WriteCouldNotFind(key);
            return;
        }

        var currentList = getCurrentList(value).ToList();
        
        foreach (var k in list)
        {
            var v = manager.Get(k);

            if (v is null)
            {
                WriteCouldNotFind<TK, TV>(k);
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

    protected void WriteSuccess(TKey key, Operations operation, TValue? value = null)
    {
        Console.WriteLine(SuccessMessage(key, operation, value));
    }

    protected void WriteFailure(TKey key, Operations operation, TValue? value = null)
    {
        Console.Error.WriteLine(FailureMessage(key, operation, value));
    }

    protected void WriteAlreadyExists(TKey key)
    {
        Console.Error.WriteLine(AlreadyExistsMessage(key));
    }

    protected void WriteCouldNotFind(TKey key)
    {
        WriteCouldNotFind<TKey, TValue>(key);
    }

    protected void WriteCouldNotFind<TK, TV>(TK key)
    {
        Console.Error.WriteLine(CouldNotFindMessage<TK, TV>(key));
    }

    private static string CouldNotFindMessage<TK, TV>(TK key)
    {
        return $"Could not find {GetEntityType(typeof(TV))} using {key}";
    }

    private static string AlreadyExistsMessage(TKey key)
    {
        return AlreadyExistsMessage(key, typeof(TValue));
    }

    private static string AlreadyExistsMessage(TKey key, Type type)
    {
        return $"Found an existing {GetEntityType(type)} using {key}";
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

    protected static string GetEntityType(Type type)
    {
        return type switch
        {
            not null when type == typeof(DeleteCondition) => "delete condition",
            not null when type == typeof(Individual) => "individual",
            not null when type == typeof(Origin) => "origin",
            not null when type == typeof(PersonalData) => "personal data",
            not null when type == typeof(PersonalDataColumn) => "personal data column",
            not null when type == typeof(Processing) => "processing",
            not null when type == typeof(Purpose) => "purpose",
            not null when type == typeof(VacuumingRule) => "vacuuming rule",
            _ => "entity"
        };
    }

    private static string GetEntityType()
    {
        return GetEntityType(typeof(TValue));
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