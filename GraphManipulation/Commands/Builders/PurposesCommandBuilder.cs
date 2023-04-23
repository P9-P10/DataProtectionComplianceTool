using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class PurposesCommandBuilder
{
    // TODO: DeleteCondition skal bare være en del af Add og Update, og så drop den specifikke command

    // TODO: Alle Add commands skal også tjekke om det der indsættes findes i forvejen
    public static Command Build(IConsole console, IPurposesManager purposesManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.PurposesName)
            .WithAlias(CommandNamer.PurposesAlias)
            .WithSubCommands(
                AddPurpose(purposesManager),
                UpdatePurpose(console, purposesManager),
                DeletePurpose(console, purposesManager),
                ListPurposes(console, purposesManager),
                ShowPurpose(console, purposesManager),
                SetDeleteCondition(console, purposesManager)
            );
    }

    private static Command AddPurpose(IPurposesManager purposesManager)
    {
        var nameOption = BuildNameOption()
            .WithIsRequired(true);

        var descriptionOption = OptionBuilder
            .CreateDescriptionOption<string>()
            .WithDescription("The description of the purpose")
            .WithGetDefaultValue(() => "");

        var legallyRequiredOption = BuildLegallyRequiredOption<bool>()
            .WithDescription("Whether the purpose falls under any legal obligations")
            .WithGetDefaultValue(() => false);

        var command = CommandBuilder
            .BuildAddCommand()
            .WithDescription("Adds a purpose to the system")
            .WithOptions(nameOption, descriptionOption, legallyRequiredOption);

        command.SetHandler(purposesManager.Add, nameOption, legallyRequiredOption, descriptionOption);

        return command;
    }

    private static Command UpdatePurpose(IConsole console, IPurposesManager purposesManager)
    {
        var nameOption = BuildNameOption()
            .WithIsRequired(true);

        var newNameOption = OptionBuilder
            .CreateNewNameOption<string>()
            .WithDescription("The new name of the purpose");

        var descriptionOption = OptionBuilder
            .CreateDescriptionOption<string>()
            .WithDescription("The new description of the purpose");

        var legallyRequiredOption = BuildLegallyRequiredOption<bool>()
            .WithDescription("The new value for if the purpose falls under any legal obligations");

        return CommandBuilder
            .BuildUpdateCommand()
            .WithDescription("Updates the purpose of the given name with the given values")
            .WithOptions(nameOption, newNameOption, descriptionOption, legallyRequiredOption)
            .WithHandler(BaseBuilder.BuildHandlerWithKey(console, purposesManager, nameOption, "purpose",
                (context, purpose) =>
                {
                    BaseBuilder.CompareAndRun(
                        context,
                        purpose.GetDescription(),
                        descriptionOption,
                        newDescription => purposesManager.UpdateDescription(purpose.GetName(), newDescription)
                    );
                },
                (context, purpose) =>
                {
                    BaseBuilder.CompareAndRun(
                        context,
                        purpose.GetLegallyRequired(),
                        legallyRequiredOption,
                        newLegallyRequired => purposesManager.UpdateLegallyRequired(purpose.GetName(), newLegallyRequired)
                    );
                },
                (context, purpose) =>
                {
                    BaseBuilder.CompareAndRun(
                        context,
                        purpose.GetName(),
                        newNameOption,
                        newName => purposesManager.UpdateName(purpose.GetName(), newName)
                    );
                })
            );
    }

    private static Command DeletePurpose(IConsole console, IPurposesManager purposesManager)
    {
        var nameOption = BuildNameOption()
            .WithIsRequired(true);

        var command = CommandBuilder
            .BuildDeleteCommand<IPurposesManager, IPurpose, string>(console, purposesManager, nameOption, "purpose")
            .WithDescription("Deletes the given purpose from the system")
            .WithOptions(nameOption);

        return command;
    }

    private static Command ListPurposes(IConsole console, IPurposesManager purposesManager)
    {
        return CommandBuilder.BuildListCommand(console, purposesManager)
            .WithDescription("Lists the purposes currently in the system");
    }

    private static Command ShowPurpose(IConsole console, IPurposesManager purposesManager)
    {
        var nameOption = BuildNameOption()
            .WithIsRequired(true);

        return CommandBuilder
            .BuildShowCommand(console, purposesManager, nameOption, "purpose")
            .WithDescription("Shows details about the given purpose")
            .WithOptions(nameOption);
    }

    private static Command SetDeleteCondition(IConsole console, IPurposesManager purposesManager)
    {
        var purposeNameOption = OptionBuilder
            .CreateOption<string>("--purpose-name")
            .WithAlias("-pn")
            .WithDescription("The purpose that receives a delete condition")
            .WithIsRequired(true);

        var deleteConditionOption = OptionBuilder
            .CreateOption<string>("--delete-condition-name")
            .WithAlias("-dcn")
            .WithDescription("The delete condition that the purpose receives")
            .WithIsRequired(true);

        var command = CommandBuilder
            .BuildSetCommand("delete-condition")
            .WithDescription("Sets the delete condition of the given purpose")
            .WithOptions(purposeNameOption, deleteConditionOption);

        command.SetHandler((purposeName, deleteCondition) =>
        {
            var purpose = purposesManager.Get(purposeName);

            if (purpose is null)
            {
                console.WriteLine(CommandBuilder.BuildFailureToFindMessage("purpose", purposeName));
                return;
            }

            // TODO: Det skal også tjekkes om den givne delete condition findes
            purposesManager.SetDeleteCondition(purposeName, deleteCondition);
        }, purposeNameOption, deleteConditionOption);

        return command;
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
}