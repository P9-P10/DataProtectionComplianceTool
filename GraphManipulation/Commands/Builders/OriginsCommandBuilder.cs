using System.CommandLine;
using GraphManipulation.Commands.Builders.Binders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders;

public class OriginsCommandBuilder : BaseCommandBuilder<string, Origin>
{
    public OriginsCommandBuilder(IManager<string, Origin> manager) : base(manager)
    {
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.OriginsName, CommandNamer.OriginsAlias, out var keyOption);

        var descriptionOption = BuildDescriptionOption();
        var newKeyOption = BuildNewNameOption();

        var createOriginBinder = new OriginBinder(keyOption, descriptionOption);
        var updateOriginBinder = new OriginBinder(newKeyOption, descriptionOption);

        return baseCommand
            .WithSubCommands(
                CreateCommand(keyOption, createOriginBinder, new Option[]
                {
                    descriptionOption
                }), 
                UpdateCommand(keyOption, updateOriginBinder, new Option[]
                {
                    newKeyOption, descriptionOption
                }) 
            );
    }

    protected override void StatusReport(Origin value)
    {
        // Nothing to report on
    }

    protected override Option<string> BuildKeyOption()
    {
        return base.BuildKeyOption(OptionNamer.Name, OptionNamer.NameAlias, "The name of the origin");
    }
}