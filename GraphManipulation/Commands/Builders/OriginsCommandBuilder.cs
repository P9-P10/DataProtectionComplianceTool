using System.CommandLine;
using GraphManipulation.Commands.Builders.Binders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders;

public class OriginsCommandBuilder : BaseCommandBuilder<IOriginsManager, string, Origin>
{
    public OriginsCommandBuilder(IConsole console, IOriginsManager manager) : base(console, manager)
    {
    }

    public Command Build()
    {
        const string subject = "origin";
        const string subjects = "origins";

        var keyOption = BuildKeyOption();

        var descriptionOption = OptionBuilder
            .CreateDescriptionOption()
            .WithDescription("The description of the origin");

        var newKeyOption = OptionBuilder
            .CreateNewNameOption()
            .WithDescription("The new name of the origin");

        return CommandBuilder.CreateCommand(CommandNamer.OriginsName)
            .WithAlias(CommandNamer.OriginsAlias)
            .WithSubCommands(
                CreateCommand(subject, keyOption, new OriginBinder(keyOption, descriptionOption), descriptionOption), 
                UpdateCommand(subject, keyOption, new OriginBinder(newKeyOption, descriptionOption), newKeyOption, descriptionOption), 
                DeleteCommand(subject, BuildKeyOption()),
                ListCommand(subjects),
                ShowCommand(subject, BuildKeyOption())
            );
    }

    private Option<string> BuildKeyOption()
    {
        return base.BuildKeyOption(OptionNamer.Name, OptionNamer.NameAlias, "The name of the origin");
    }
}