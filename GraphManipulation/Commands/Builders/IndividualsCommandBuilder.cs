using System.CommandLine;
using GraphManipulation.Commands.Builders.Binders;
using GraphManipulation.Commands.Factories;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders;

public class IndividualsCommandBuilder : BaseCommandBuilder<int, Individual>
{

    public IndividualsCommandBuilder(IHandlerFactory handlerFactory, IManagerFactory managerFactory) : base(handlerFactory)
    {
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.IndividualsName, CommandNamer.IndividualsAlias, out var keyOption);

        var descriptionOption = OptionBuilder.CreateEntityDescriptionOption<Individual>();

        return baseCommand
            .WithSubCommands(
                CreateCommand(keyOption, new IndividualBinder(keyOption, descriptionOption), new Option[]
                {
                    
                })
            );
    }

    protected override void StatusReport(Individual value)
    {
        // TODO: Missing origin for X personal data column
    }

    protected override Option<int> BuildKeyOption()
    {
        return OptionBuilder.CreateKeyOption<int, Individual>(OptionNamer.Id, OptionNamer.IdAlias, "id");
    }
}