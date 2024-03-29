using System.CommandLine;
using GraphManipulation.Commands.Binders;
using GraphManipulation.Factories.Interfaces;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Builders;

public class ProcessingCommandBuilder : BaseCommandBuilder<string, Processing>
{
    private readonly IManager<TableColumnPair, PersonalDataColumn> _personalDataColumnManager;
    private readonly IManager<string, Purpose> _purposesManager;

    public ProcessingCommandBuilder(ICommandHandlerFactory commandHandlerFactory, IManagerFactory managerFactory) :
        base(commandHandlerFactory)
    {
        _purposesManager = managerFactory.CreateManager<string, Purpose>();
        _personalDataColumnManager = managerFactory.CreateManager<TableColumnPair, PersonalDataColumn>();
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.ProcessingName, CommandNamer.ProcessingAlias, out var keyOption);

        var descriptionOption = OptionBuilder.CreateEntityDescriptionOption<Processing>();
        var newKeyOption = OptionBuilder.CreateNewNameOption<Processing>();

        var tableColumnOption = OptionBuilder
            .CreateTableColumnPairOption()
            .WithDescription("The personal data that is being processed");

        var purposeOption = OptionBuilder
            .CreateOption<string>(OptionNamer.Purpose)
            .WithAlias(OptionNamer.PurposeAlias)
            .WithDescription("The purpose of the processing");

        var createBinder = new ProcessingBinder(
            keyOption,
            descriptionOption,
            purposeOption,
            tableColumnOption,
            _purposesManager,
            _personalDataColumnManager
        );

        var updateBinder = new ProcessingBinder(
            newKeyOption,
            descriptionOption,
            purposeOption,
            tableColumnOption,
            _purposesManager,
            _personalDataColumnManager
        );

        return baseCommand
            .WithSubCommands(
                CreateCommand(keyOption, createBinder, new Option[]
                {
                    descriptionOption, purposeOption, tableColumnOption
                }),
                UpdateCommand(keyOption, updateBinder, new Option[]
                {
                    newKeyOption, descriptionOption, purposeOption, tableColumnOption
                })
            );
    }

    protected override void StatusReport(Processing processing)
    {
        if (processing.Purpose is null)
        {
            FeedbackEmitter.EmitMissing<Purpose>(processing.Key!);
        }

        if (processing.PersonalDataColumn is null)
        {
            FeedbackEmitter.EmitMissing<PersonalDataColumn>(processing.Key!);
        }
    }

    protected override Option<string> BuildKeyOption()
    {
        return OptionBuilder.CreateKeyOption<string, Processing>(OptionNamer.Name, OptionNamer.NameAlias);
    }
}