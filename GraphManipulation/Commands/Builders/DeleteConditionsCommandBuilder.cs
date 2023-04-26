using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class DeleteConditionsCommandBuilder
{
    public static Command Build(IConsole console, IDeleteConditionsManager deleteConditionsManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.DeleteConditionName)
            .WithAlias(CommandNamer.DeleteConditionAlias)
            .WithSubCommands(
                AddDeleteCondition(console, deleteConditionsManager),
                UpdateDeleteCondition(console, deleteConditionsManager),
                DeleteDeleteCondition(console, deleteConditionsManager),
                ListDeleteConditions(console, deleteConditionsManager),
                ShowDeleteCondition(console, deleteConditionsManager)
            );
    }

    private static Command AddDeleteCondition(IConsole console, IDeleteConditionsManager deleteConditionsManager)
    {
        return CommandBuilder
            .BuildAddCommand()
            .WithDescription("Adds a delete condition to the system")
            .WithOption(out var nameOption, BuildNameOption())
            .WithOption(out var descriptionOption,
                OptionBuilder
                    .CreateDescriptionOption()
                    .WithDescription("The description of the delete condition")
                    .WithGetDefaultValue(() => ""))
            .WithOption(out var conditionOption,
                BuildConditionOption<string>()
                    .WithDescription("The delete condition itself")
                    .WithIsRequired(true))
            .WithHandler(context =>
            {
                Handlers.AddHandler(context, console,
                    deleteConditionsManager.Add,
                    deleteConditionsManager,
                    nameOption,
                    descriptionOption,
                    conditionOption);
            });

        // var nameOption = BuildNameOption()
        //     .WithIsRequired(true);
        //
        // var descriptionOption = OptionBuilder
        //     .CreateDescriptionOption<string>()
        //     .WithDescription("The description of the delete condition")
        //     .WithGetDefaultValue(() => "");
        //
        // var conditionOption = BuildConditionOption<string>()
        //     .WithDescription("The delete condition itself")
        //     .WithIsRequired(true);
        //
        // var command = CommandBuilder
        //     .BuildAddCommand()
        //     .WithDescription("Adds a delete condition to the system")
        //     .WithOptions(nameOption, conditionOption, descriptionOption);

        // command.SetHandler(deleteConditionsManager.Add, nameOption, descriptionOption, conditionOption);
        //
        // return command;
    }

    private static Command UpdateDeleteCondition(IConsole console, IDeleteConditionsManager deleteConditionsManager)
    {
        return CommandBuilder
            .BuildUpdateCommand()
            .WithDescription("Updates the delete condition of the given name with the given values")
            .WithOption(out var nameOption, BuildNameOption())
            .WithOption(out var newNameOption,
                OptionBuilder
                    .CreateNewNameOption()
                    .WithDescription("The new name of the delete condition"))
            .WithOption(out var descriptionOption,
                OptionBuilder
                    .CreateDescriptionOption()
                    .WithDescription("The new description of the delete condition"))
            .WithOption(out var conditionOption,
                BuildConditionOption<string>()
                    .WithDescription("The new condition"))
            .WithHandler(context =>
            {
                Handlers.UpdateHandler(context, console,
                    deleteConditionsManager.UpdateDescription,
                    deleteConditionsManager,
                    d => d.GetDescription(),
                    nameOption,
                    descriptionOption);

                Handlers.UpdateHandler(context, console,
                    deleteConditionsManager.UpdateCondition,
                    deleteConditionsManager,
                    d => d.GetCondition(),
                    nameOption,
                    conditionOption);

                Handlers.UpdateHandler(context, console,
                    deleteConditionsManager.UpdateName,
                    deleteConditionsManager,
                    d => d.GetName(),
                    nameOption,
                    newNameOption);
            });

        // var nameOption = BuildNameOption()
        //     .WithIsRequired(true);
        //
        // var newNameOption = OptionBuilder
        //     .CreateNewNameOption<string?>()
        //     .WithDescription("The new name of the delete condition");
        //
        // var descriptionOption = OptionBuilder
        //     .CreateDescriptionOption<string?>()
        //     .WithDescription("The new description of the delete condition");
        //
        // var conditionOption = BuildConditionOption<string?>()
        //     .WithDescription("The new condition");
        //
        // var command = CommandBuilder
        //     .BuildUpdateCommand()
        //     .WithDescription("Updates the delete condition of the given name with the given values")
        //     .WithOptions(nameOption, newNameOption, conditionOption, descriptionOption);
        //
        // command.SetHandler((name, newName, condition, description) =>
        // {
        //     var old = deleteConditionsManager.Get(name);
        //
        //     if (old == null)
        //     {
        //         console.WriteLine(CommandBuilder.BuildFailureToFindMessage("delete condition", name));
        //         return;
        //     }
        //
        //     if (description is not null && old.GetDescription() != description)
        //     {
        //         deleteConditionsManager.UpdateDescription(name, description);
        //     }
        //
        //     if (condition is not null && old.GetCondition() != condition)
        //     {
        //         deleteConditionsManager.UpdateCondition(name, condition);
        //     }
        //
        //     if (newName is not null && name != newName)
        //     {
        //         deleteConditionsManager.UpdateName(name, newName);
        //     }
        // }, nameOption, newNameOption, conditionOption, descriptionOption);
        //
        // return command;
    }

    private static Command DeleteDeleteCondition(IConsole console, IDeleteConditionsManager deleteConditionsManager)
    {
        return CommandBuilder
            .BuildDeleteCommand()
            .WithDescription("Deletes the given delete condition from the system")
            .WithOption(out var nameOption, BuildNameOption())
            .WithHandler(context => Handlers.DeleteHandler(context, console,
                deleteConditionsManager.Delete,
                deleteConditionsManager,
                nameOption));
    }

    private static Command ListDeleteConditions(IConsole console, IDeleteConditionsManager deleteConditionsManager)
    {
        return CommandBuilder
            .BuildListCommand()
            .WithDescription("Lists the delete conditions currently in the system")
            .WithHandler(() => Handlers.ListHandler(console, deleteConditionsManager));
    }

    private static Command ShowDeleteCondition(IConsole console, IDeleteConditionsManager deleteConditionsManager)
    {
        return CommandBuilder
            .BuildShowCommand()
            .WithDescription("Shows details about the given delete condition")
            .WithOption(out var nameOption, BuildNameOption())
            .WithHandler(context => Handlers.ShowHandler(context, console, deleteConditionsManager, nameOption));
    }

    private static Option<string> BuildNameOption()
    {
        return OptionBuilder
            .CreateNameOption()
            .WithDescription("The name of the delete condition")
            .WithIsRequired(true);
    }

    private static Option<T> BuildConditionOption<T>()
    {
        return OptionBuilder
            .CreateOption<T>("--condition")
            .WithAlias("-c");
    }
}