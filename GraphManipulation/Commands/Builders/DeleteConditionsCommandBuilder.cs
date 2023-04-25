using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class DeleteConditionsCommandBuilder
{
    public static Command Build(IConsole console, IDeleteConditionsManager deleteConditionsManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.DeleteConditionName)
            .WithAlias(CommandNamer.DeleteConditionAlias)
            .WithSubCommands(
                AddDeleteCondition(deleteConditionsManager),
                UpdateDeleteCondition(console, deleteConditionsManager),
                DeleteDeleteCondition(console, deleteConditionsManager),
                ListDeleteConditions(console, deleteConditionsManager),
                ShowDeleteCondition(console, deleteConditionsManager)
            );
    }

    private static Command AddDeleteCondition(IDeleteConditionsManager deleteConditionsManager)
    {
        var nameOption = BuildNameOption()
            .WithIsRequired(true);

        var descriptionOption = OptionBuilder
            .CreateDescriptionOption<string>()
            .WithDescription("The description of the delete condition")
            .WithGetDefaultValue(() => "");

        var conditionOption = BuildConditionOption<string>()
            .WithDescription("The delete condition itself")
            .WithIsRequired(true);

        var command = CommandBuilder
            .BuildAddCommand()
            .WithDescription("Adds a delete condition to the system")
            .WithOptions(nameOption, conditionOption, descriptionOption);

        command.SetHandler(deleteConditionsManager.Add, nameOption, descriptionOption, conditionOption);

        return command;
    }

    private static Command UpdateDeleteCondition(IConsole console, IDeleteConditionsManager deleteConditionsManager)
    {
        var nameOption = BuildNameOption()
            .WithIsRequired(true);

        var newNameOption = OptionBuilder
            .CreateNewNameOption<string?>()
            .WithDescription("The new name of the delete condition");

        var descriptionOption = OptionBuilder
            .CreateDescriptionOption<string?>()
            .WithDescription("The new description of the delete condition");

        var conditionOption = BuildConditionOption<string?>()
            .WithDescription("The new condition");

        var command = CommandBuilder
            .BuildUpdateCommand()
            .WithDescription("Updates the delete condition of the given name with the given values")
            .WithOptions(nameOption, newNameOption, conditionOption, descriptionOption);

        command.SetHandler((name, newName, condition, description) =>
        {
            var old = deleteConditionsManager.Get(name);

            if (old == null)
            {
                console.WriteLine(CommandBuilder.BuildFailureToFindMessage("delete condition", name));
                return;
            }

            if (description is not null && old.GetDescription() != description)
            {
                deleteConditionsManager.UpdateDescription(name, description);
            }

            if (condition is not null && old.GetCondition() != condition)
            {
                deleteConditionsManager.UpdateCondition(name, condition);
            }

            if (newName is not null && name != newName)
            {
                deleteConditionsManager.UpdateName(name, newName);
            }
        }, nameOption, newNameOption, conditionOption, descriptionOption);

        return command;
    }

    private static Command DeleteDeleteCondition(IConsole console, IDeleteConditionsManager deleteConditionsManager)
    {
        var nameOption = BuildNameOption()
            .WithIsRequired(true);

        var command = CommandBuilder
            .BuildDeleteCommand<IDeleteConditionsManager, IDeleteCondition, string>(console, deleteConditionsManager,
                nameOption, "delete condition")
            .WithDescription("Deletes the given delete condition from the system")
            .WithOptions(nameOption);

        return command;
    }

    private static Command ListDeleteConditions(IConsole console, IDeleteConditionsManager deleteConditionsManager)
    {
        return CommandBuilder.BuildListCommand(console, deleteConditionsManager)
            .WithDescription("Lists the delete conditions currently in the system");
    }

    private static Command ShowDeleteCondition(IConsole console, IDeleteConditionsManager deleteConditionsManager)
    {
        var nameOption = BuildNameOption()
            .WithIsRequired(true);

        return CommandBuilder
            .BuildShowCommand(console, deleteConditionsManager, nameOption, "delete condition")
            .WithDescription("Shows details about the given delete condition")
            .WithOptions(nameOption);
    }

    private static Option<string> BuildNameOption()
    {
        return OptionBuilder
            .CreateNameOption()
            .WithDescription("The name of the delete condition");
    }

    private static Option<T> BuildConditionOption<T>()
    {
        return OptionBuilder
            .CreateOption<T>("--condition")
            .WithAlias("-c");
    }
}