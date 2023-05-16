using System.CommandLine;
using GraphManipulation.Commands.Builders.Binders;
using GraphManipulation.Commands.Factories;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders;

public class OriginsCommandBuilder : BaseCommandBuilder<string, Origin>
{
    public OriginsCommandBuilder(IHandlerFactory handlerFactory) : base(handlerFactory)
    {
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.OriginsName, CommandNamer.OriginsAlias, out var keyOption);

        var descriptionOption = OptionBuilder.CreateEntityDescriptionOption<Origin>();
        var newKeyOption = OptionBuilder.CreateNewNameOption<Origin>();

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
        return OptionBuilder.CreateKeyOption<string, Origin>(OptionNamer.Name, OptionNamer.NameAlias);
    }
}