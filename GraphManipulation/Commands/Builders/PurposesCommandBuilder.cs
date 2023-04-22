using System.CommandLine;
using System.CommandLine.Parsing;
using GraphManipulation.Commands.BaseBuilders;
using GraphManipulation.Managers.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class PurposesCommandBuilder
{
    // TODO: DeleteCondition skal bare være en del af Add og Update, og så drop den specifikke command
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

        command.SetHandler(
            (name, description, legallyRequired) => purposesManager.Add(name, legallyRequired, description),
            nameOption, descriptionOption, legallyRequiredOption);

        return command;
    }

    private static Command UpdatePurpose(IConsole console, IPurposesManager purposesManager)
    {
        var nameOption = BuildNameOption()
            .WithIsRequired(true);

        var newNameOption = OptionBuilder
            .CreateOption<string?>("--new-name")
            .WithAlias("-nn")
            .WithDescription("The new name of the purpose");

        var descriptionOption = OptionBuilder
            .CreateDescriptionOption<string?>()
            .WithDescription("The new description of the purpose");

        var legallyRequiredOption = BuildLegallyRequiredOption<bool?>()
            .WithDescription("The new value for if the purpose falls under any legal obligations");

        var command = CommandBuilder
            .BuildUpdateCommand()
            .WithDescription("Updates the purpose of the given name with the given values")
            .WithOptions(
                nameOption,
                newNameOption,
                descriptionOption,
                legallyRequiredOption
            );
        
        command.SetHandler((name, newName, description, legallyRequired) =>
        {
            var old = purposesManager.Get(name);

            if (old == null)
            {
                console.WriteLine($"Could not find a purpose using \"{name}\"");
                return;
            }

            if (description is not null && old.GetDescription() != description)
            {
                purposesManager.UpdateDescription(name, description);
            }

            if (legallyRequired is not null && old.GetLegallyRequired() != legallyRequired)
            {
                purposesManager.UpdateLegallyRequired(name, legallyRequired.Value);
            }

            if (newName is not null && name != newName)
            {
                purposesManager.UpdateName(name, newName);
            }

        }, nameOption, newNameOption, descriptionOption, legallyRequiredOption);

        return command;
    }

    private static Command DeletePurpose(IConsole console, IPurposesManager purposesManager)
    {
        var nameOption = BuildNameOption()
            .WithIsRequired(true);

        var command = CommandBuilder
            .BuildDeleteCommand()
            .WithDescription("Deletes the given purpose from the system")
            .WithOptions(nameOption);

        command.SetHandler(name =>
        {
            var purpose = purposesManager.Get(name);

            if (purpose is null)
            {
                console.WriteLine($"Could not find a purpose using \"{name}\"");
                return;
            }
            
            purposesManager.Delete(name);
            
        }, nameOption);
        
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
                console.WriteLine($"Could not find a purpose using {purposeName}");
            }
            
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