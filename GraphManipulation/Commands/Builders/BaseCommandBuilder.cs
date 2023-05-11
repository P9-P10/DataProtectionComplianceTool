using System.CommandLine;
using System.CommandLine.IO;
using GraphManipulation.Commands.Builders.Binders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Models.Base;
using GraphManipulation.Models.Interfaces.Base;

namespace GraphManipulation.Commands.Builders;

public abstract class BaseCommandBuilder<TManager, TKey, TValue>
    where TKey : notnull
    where TValue : Entity<TKey>, IListable, new()
    where TManager : IManager<TKey, TValue>
{
    protected readonly TManager Manager;
    protected readonly IConsole Console;

    protected BaseCommandBuilder(IConsole console, TManager manager)
    {
        Console = console;
        Manager = manager;
    }

    protected void CreateHandler(TKey key, TValue value)
    {
        if (Manager.Get(key) is not null)
        {
            // Cannot create something that exists already
            AlreadyExists(key);
            return;
        }

        if (Manager.Create(key))
        {
            Success(key, Operations.Created);
            UpdateHandler(key, value);
        }
        else
        {
            // Could not create entity
            Failure(key, Operations.Created);
        }
    }

    protected void UpdateHandler(TKey key, TValue value)
    {
        var old = Manager.Get(key);

        if (old is null)
        {
            // Can only update something that exists
            CouldNotFind(key);
            return;
        }

        if (!key.Equals(value.Key!) && Manager.Get(value.Key!) is not null)
        {
            // If the key is updated, it can only be updated to something that doesn't already exist
            AlreadyExists(value.Key!);
            return;
        }

        old.Fill(value);

        if (Manager.Update(key, value))
        {
            Success(key, Operations.Updated, value);
        }
        else
        {
            Failure(key, Operations.Updated, value);
        }
    }

    private void DeleteHandler(TKey key)
    {
        if (Manager.Get(key) is null)
        {
            // Can only delete something that exists
            CouldNotFind(key);
            return;
        }

        if (Manager.Delete(key))
        {
            Success(key, Operations.Deleted);
        }
        else
        {
            Failure(key, Operations.Deleted);
        }
    }

    private void ShowHandler(TKey key)
    {
        if (Manager.Get(key) is null)
        {
            // Can only show something that exists
            CouldNotFind(key);
            return;
        }

        Console.WriteLine(Manager.Get(key)!.ToListing());
    }

    private void ListHandler()
    {
        Manager.GetAll().Select(r => r.ToListing()).ToList().ForEach(Console.WriteLine);
    }

    protected Command ListCommand()
    {
        var command = CommandBuilder
            .BuildListCommand()
            .WithDescription($"Lists the {GetEntityType()}(e)s currently in the system");

        command.SetHandler(ListHandler);
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

    protected Command DeleteCommand(Option<TKey> keyOption)
    {
        var command = CommandBuilder
            .BuildDeleteCommand()
            .WithDescription($"Deletes the given {GetEntityType()} from the system")
            .WithOption(out _, keyOption);

        command.SetHandler(DeleteHandler, keyOption);

        return command;
    }

    protected Command CreateCommand(Option<TKey> keyOption, BaseBinder<TKey, TValue> binder,
        params Option[] options)
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
        params Option[] options)
    {
        var command = CommandBuilder
            .BuildUpdateCommand()
            .WithDescription($"Creates a new {GetEntityType()} in the system")
            .WithOption(out _, keyOption)
            .WithOptions(options);

        command.SetHandler(UpdateHandler, keyOption, binder);

        return command;
    }

    protected Option<TKey> BuildKeyOption(string name, string alias, string description)
    {
        return OptionBuilder
            .CreateOption<TKey>(name)
            .WithAlias(alias)
            .WithDescription(description)
            .WithIsRequired(true);
    }

    protected void Success(TKey key, Operations operation, TValue? value = null)
    {
        Console.WriteLine(SuccessMessage(key, operation, value));
    }

    protected void Failure(TKey key, Operations operation, TValue? value = null)
    {
        Console.Error.WriteLine(FailureMessage(key, operation, value));
    }

    protected void AlreadyExists(TKey key)
    {
        Console.Error.WriteLine(AlreadyExistsMessage(key));
    }

    protected void CouldNotFind(TKey key)
    {
        Console.Error.WriteLine(CouldNotFindMessage(key));
    }
    
    protected static string CouldNotFindMessage(TKey key)
    {
        return $"Could not find {GetEntityType()} using {key}";
    }

    protected static string AlreadyExistsMessage(TKey key)
    {
        return AlreadyExistsMessage(key, typeof(TValue));
    }

    protected static string AlreadyExistsMessage(TKey key, Type type)
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

    protected static string GetEntityType()
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