using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders;

public class IndividualsCommandBuilder : BaseCommandBuilder<int, Individual>
{
    private readonly IManager<TableColumnPair, PersonalDataColumn> _personalDataColumnManager;
    private readonly IManager<string, Origin> _originsManager;

    public IndividualsCommandBuilder(
        IManager<int, Individual> manager,
        IManager<TableColumnPair, PersonalDataColumn> personalDataColumnManager,
        IManager<string, Origin> originsManager) : base( manager)
    {
        _personalDataColumnManager = personalDataColumnManager;
        _originsManager = originsManager;
    }

    public override Command Build()
    {
        var command = CommandBuilder
            .CreateNewCommand(CommandNamer.IndividualsName)
            .WithAlias(CommandNamer.IndividualsAlias)
            .WithSubCommands(
                ListCommand(),
                SetOriginForCommand(),
                ShowOriginForCommand(),
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
    
    // TODO: Implementer SetOriginForCommand og ShowOriginForCommand

    protected Command SetOriginForCommand()
    {
        throw new NotImplementedException();
    }

    protected Command ShowOriginForCommand()
    {
        throw new NotImplementedException();
    }
}