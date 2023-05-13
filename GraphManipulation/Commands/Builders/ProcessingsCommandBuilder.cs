using System.CommandLine;
using GraphManipulation.Commands.Builders.Binders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders;

public class ProcessingsCommandBuilder : BaseCommandBuilder<string, Processing>
{
    private readonly IManager<string, Purpose> _purposesManager;
    private readonly IManager<TableColumnPair, PersonalDataColumn> _personalDataColumnManager;
    
    public ProcessingsCommandBuilder(
        IManager<string, Processing> manager, 
        IManager<string, Purpose> purposesManager, 
        IManager<TableColumnPair, PersonalDataColumn> personalDataColumnManager) : base(manager)
    {
        _purposesManager = purposesManager;
        _personalDataColumnManager = personalDataColumnManager;
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.ProcessingsName, CommandNamer.ProcessingsAlias, out var keyOption);

        var descriptionOption = BuildDescriptionOption();
        var newKeyOption = BuildNewNameOption();
        
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
                    descriptionOption, purposeOption.WithIsRequired(true), tableColumnOption.WithIsRequired(true)
                }),
                UpdateCommand(keyOption, updateBinder, new Option[]
                {
                    newKeyOption, descriptionOption, purposeOption, tableColumnOption
                })
            );
    }

    protected override void StatusReport(Processing value)
    {
        // Nothing to report on
    }

    protected override Option<string> BuildKeyOption()
    {
        return base.BuildKeyOption(OptionNamer.Name, OptionNamer.NameAlias, "The name of the processing");
    }
}