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
        var baseCommand = base.Build(CommandNamer.OriginsName, CommandNamer.OriginsAlias);
        
        var keyOption = BuildKeyOption();
        
        var descriptionOption = OptionBuilder
            .CreateDescriptionOption()
            .WithDescription("The description of the origin");

        var newKeyOption = OptionBuilder
            .CreateNewNameOption()
            .WithDescription("The new name of the origin");

        return baseCommand
            .WithSubCommands(
                BaseCreateCommand(keyOption, new OriginBinder(keyOption, descriptionOption), descriptionOption), 
                BaseUpdateCommand(keyOption, new OriginBinder(newKeyOption, descriptionOption), newKeyOption, descriptionOption) 
            );
    }

    protected override Option<string> BuildKeyOption()
    {
        return base.BuildKeyOption(OptionNamer.Name, OptionNamer.NameAlias, "The name of the origin");
    }
}