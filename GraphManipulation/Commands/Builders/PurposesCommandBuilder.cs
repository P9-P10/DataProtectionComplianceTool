using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class PurposesCommandBuilder
{
    public static Command Build(IConsole console, IPurposesManager purposesManager,
        IDeleteConditionsManager deleteConditionsManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.PurposesName)
            .WithAlias(CommandNamer.PurposesAlias)
            .WithSubCommands(
                AddPurpose(console, purposesManager, deleteConditionsManager),
                UpdatePurpose(console, purposesManager, deleteConditionsManager),
                DeletePurpose(console, purposesManager),
                ListPurposes(console, purposesManager),
                ShowPurpose(console, purposesManager)
            );
    }

    private static Command AddPurpose(IConsole console, IPurposesManager purposesManager,
        IDeleteConditionsManager deleteConditionsManager)
    {
        return CommandBuilder
            .BuildAddCommand()
            .WithDescription("Adds a purpose to the system")
            .WithOption(out var nameOption,
                BuildNameOption()
                    .WithIsRequired(true))
            .WithOption(out var descriptionOption,
                OptionBuilder
                    .CreateDescriptionOption()
                    .WithDescription("The description of the purpose")
                    .WithGetDefaultValue(() => ""))
            .WithOption(out var legallyRequiredOption,
                BuildLegallyRequiredOption<bool>()
                    .WithDescription("Whether the purpose falls under any legal obligations")
                    .WithGetDefaultValue(() => false))
            .WithOption(out var deleteConditionOption,
                BuildDeleteConditionOption()
                    .WithDescription("The delete condition that the purpose receives"))
            .WithHandler(context =>
            {
                Handlers.AddHandler(context, console,
                    purposesManager.Add,
                    purposesManager,
                    nameOption,
                    legallyRequiredOption,
                    descriptionOption);

                Handlers.UpdateHandlerWithKey(context, console,
                    purposesManager.SetDeleteCondition,
                    purposesManager,
                    deleteConditionsManager,
                    purpose => purpose.GetDeleteCondition(),
                    nameOption, deleteConditionOption);
            });
    }

    private static Command UpdatePurpose(IConsole console, IPurposesManager purposesManager,
        IDeleteConditionsManager deleteConditionsManager)
    {
        return CommandBuilder
            .BuildUpdateCommand()
            .WithDescription("Updates the purpose of the given name with the given values")
            .WithOption(out var nameOption,
                BuildNameOption()
                    .WithIsRequired(true))
            .WithOption(out var newNameOption,
                OptionBuilder
                    .CreateNewNameOption()
                    .WithDescription("The new name of the purpose"))
            .WithOption(out var descriptionOption,
                OptionBuilder
                    .CreateDescriptionOption()
                    .WithDescription(
                        "The new description of the purpose (Multiple words should be encased with \" \")"))
            .WithOption(out var legallyRequiredOption,
                BuildLegallyRequiredOption<bool>()
                    .WithDescription("The new value for if the purpose falls under any legal obligations"))
            .WithOption(out var deleteConditionOption,
                BuildDeleteConditionOption()
                    .WithDescription("The new delete condition that the purpose receives"))
            .WithHandler(context =>
            {
                Handlers.UpdateHandler(context, console,
                    purposesManager.UpdateDescription,
                    purposesManager,
                    p => p.GetDescription(),
                    nameOption,
                    descriptionOption);

                Handlers.UpdateHandler(context, console,
                    purposesManager.UpdateLegallyRequired,
                    purposesManager,
                    p => p.GetLegallyRequired(),
                    nameOption,
                    legallyRequiredOption);

                Handlers.UpdateHandlerWithKey(context, console,
                    purposesManager.SetDeleteCondition,
                    purposesManager,
                    deleteConditionsManager,
                    p => p.GetDeleteCondition(),
                    nameOption,
                    deleteConditionOption);

                Handlers.UpdateHandler(context, console,
                    purposesManager.UpdateName,
                    purposesManager,
                    p => p.GetName(),
                    nameOption,
                    newNameOption);
            });
    }

    private static Command DeletePurpose(IConsole console, IPurposesManager purposesManager)
    {
        return CommandBuilder
            .BuildDeleteCommand()
            .WithDescription("Deletes the given purpose from the system")
            .WithOption(
                out var nameOption,
                BuildNameOption().WithIsRequired(true))
            .WithHandler(context => Handlers.DeleteHandler(context, console,
                purposesManager.Delete,
                purposesManager,
                nameOption)
            );
    }

    private static Command ListPurposes(IConsole console, IPurposesManager purposesManager)
    {
        return CommandBuilder
            .BuildListCommand()
            .WithDescription("Lists the purposes currently in the system")
            .WithHandler(() => Handlers.ListHandler(console, purposesManager, CommandHeader.PurposesHeader));
    }

    private static Command ShowPurpose(IConsole console, IPurposesManager purposesManager)
    {
        return CommandBuilder
            .BuildShowCommand()
            .WithDescription("Shows details about the given purpose")
            .WithOption(out var nameOption, BuildNameOption().WithIsRequired(true))
            .WithHandler(context => Handlers.ShowHandler(context, console, purposesManager, nameOption));
    }

    private static Option<string> BuildNameOption()
    {
        return OptionBuilder
            .CreateNameOption()
            .WithDescription("The name of the purpose");
    }

    private static Option<T> BuildLegallyRequiredOption<T>()
    {
        return OptionBuilder
            .CreateOption<T>("--legally-required")
            .WithAlias("-lr");
    }

    private static Option<string> BuildDeleteConditionOption()
    {
        return OptionBuilder
            .CreateOption<string>("--delete-condition-name")
            .WithAlias("-dcn");
    }
}