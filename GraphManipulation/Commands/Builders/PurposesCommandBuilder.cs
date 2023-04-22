using System.CommandLine;
using System.CommandLine.Parsing;
using GraphManipulation.Commands.BaseBuilders;
using GraphManipulation.Managers.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class PurposesCommandBuilder
{
    public static Command Build(IConsole console, IPurposesManager purposesManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.PurposesName)
            .WithAlias(CommandNamer.PurposesAlias)
            .WithSubCommands(
                AddPurpose(purposesManager),
                UpdatePurpose(console, purposesManager),
                DeletePurpose(console, purposesManager),
                ListPurposes(console, purposesManager)
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