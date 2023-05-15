using System.CommandLine;
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
        var keyOption = BuildKeyOption();
     
        var command = CommandBuilder
            .CreateNewCommand(CommandNamer.IndividualsName)
            .WithAlias(CommandNamer.IndividualsAlias)
            .WithSubCommands(
                ListCommand(),
                ShowCommand(keyOption),
                StatusCommand()
            );

        return command;
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