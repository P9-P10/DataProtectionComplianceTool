using System.CommandLine;
using GraphManipulation.Commands.Builders.Binders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;
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
            // Can not create something that exists already
            return;
        }

        if (Manager.Create(key))
        {
            Console.WriteLine($"Created entity using {key}");

            UpdateHandler(key, value);
        }
        else
        {
            // Could not create entity
        }
    }

    protected void UpdateHandler(TKey key, TValue value)
    {
        var old = Manager.Get(key);

        if (old is null)
        {
            // Can only update something that exists
            return;
        }

        if (!key.Equals(value.Key!) && Manager.Get(value.Key!) is not null)
        {
            // If the key is updated, it can only be updated to something that doesn't already exist
            return;
        }

        old.Fill(value);

        Manager.Update(key, value);
    }

    private void DeleteHandler(TKey key)
    {
        if (Manager.Get(key) is null)
        {
            // Can only delete something that exists
            return;
        }

        Manager.Delete(key);
    }

    private void ShowHandler(TKey key)
    {
        if (Manager.Get(key) is null)
        {
            // Can only show something that exists
            return;
        }

        Console.WriteLine(Manager.Get(key)!.ToListing());
    }

    private void ListHandler()
    {
        Manager.GetAll().Select(r => r.ToListing()).ToList().ForEach(Console.WriteLine);
    }

    protected Command ListCommand(string subjects)
    {
        var command = CommandBuilder
            .BuildListCommand()
            .WithDescription($"Lists the {subjects} currently in the system");

        command.SetHandler(ListHandler);
        return command;
    }

    protected Command ShowCommand(string subject, Option<TKey> keyOption)
    {
        var command = CommandBuilder
            .BuildShowCommand()
            .WithDescription($"Shows details about the given {subject}")
            .WithOption(out _, keyOption);

        command.SetHandler(ShowHandler, keyOption);

        return command;
    }

    protected Command DeleteCommand(string subject, Option<TKey> keyOption)
    {
        var command = CommandBuilder
            .BuildDeleteCommand()
            .WithDescription($"Deletes the given {subject} from the system")
            .WithOption(out _, keyOption);

        command.SetHandler(DeleteHandler, keyOption);

        return command;
    }

    protected Command CreateCommand(string subject, Option<TKey> keyOption, BaseBinder<TKey, TValue> binder,
        params Option[] options)
    {
        var command = CommandBuilder
            .BuildCreateCommand()
            .WithDescription($"Creates a new {subject} in the system")
            .WithOption(out _, keyOption)
            .WithOptions(options);

        command.SetHandler(CreateHandler, keyOption, binder);

        return command;
    }

    protected Command UpdateCommand(string subject, Option<TKey> keyOption, BaseBinder<TKey, TValue> binder,
        params Option[] options)
    {
        var command = CommandBuilder
            .BuildUpdateCommand()
            .WithDescription($"Creates a new {subject} in the system")
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
}