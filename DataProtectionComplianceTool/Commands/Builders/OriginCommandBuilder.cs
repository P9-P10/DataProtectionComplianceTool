using System.CommandLine;
using GraphManipulation.Commands.Binders;
using GraphManipulation.Factories.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Builders;

public class OriginCommandBuilder : BaseCommandBuilder<string, Origin>
{
    public OriginCommandBuilder(ICommandHandlerFactory commandHandlerFactory) : base(commandHandlerFactory)
    {
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.OriginName, CommandNamer.OriginAlias, out var keyOption);

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