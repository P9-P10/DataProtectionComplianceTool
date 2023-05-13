using System.CommandLine;
using GraphManipulation.Commands.Builders.Binders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders;

public class PersonalDataColumnCommandBuilder : BaseCommandBuilder<TableColumnPair, PersonalDataColumn>
{
    private readonly IManager<string, Purpose> _purposesManager;

    public PersonalDataColumnCommandBuilder(
        IConsole console,
        IManager<TableColumnPair, PersonalDataColumn> personalDataColumnManager,
        IManager<string, Purpose> purposesManager) : base(console, personalDataColumnManager)
    {
        _purposesManager = purposesManager;
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.PersonalDataName, CommandNamer.PersonalDataAlias, out var keyOption);

        var descriptionOption = BuildDescriptionOption();

        var defaultValueOption = OptionBuilder
            .CreateOption<string>(OptionNamer.DefaultValue)
            .WithAlias(OptionNamer.DefaultValueAlias)
            .WithDescription("The default value that attributes in the column should receive upon deletion");

        var purposeListOption = OptionBuilder
            .CreatePurposeListOption()
            .WithDescription("The purpose(s) under which the personal data is stored");

        var createBinder = new PersonalDataColumnBinder(
            keyOption,
            descriptionOption,
            purposeListOption,
            defaultValueOption,
            _purposesManager);

        var updateBinder = new PersonalDataColumnBinder(
            keyOption,
            descriptionOption,
            purposeListOption,
            defaultValueOption,
            _purposesManager);

        var purposeListChangesCommands = BuildListChangesCommand(
            keyOption, purposeListOption, _purposesManager,
            column => column.Purposes ?? new List<Purpose>(),
            (column, purposes) => column.Purposes = purposes);

        return baseCommand
            .WithSubCommands(
                CreateCommand(keyOption, createBinder, new Option[]
                {
                    descriptionOption, defaultValueOption, purposeListOption
                }),
                UpdateCommand(keyOption, updateBinder, new Option[]
                {
                    descriptionOption, defaultValueOption, purposeListOption
                }),
                purposeListChangesCommands.Add,
                purposeListChangesCommands.Remove
            );
    }

    protected override Option<TableColumnPair> BuildKeyOption()
    {
        return OptionBuilder.CreateTableColumnPairOption();
    }

    protected override void StatusReport(PersonalDataColumn column)
    {
        if (column.Purposes is null || !column.Purposes.Any())
        {
            EmitMissing<Purpose>(column.Key!);
        }

        if (column.DefaultValue is null)
        {
            EmitMissing(column.Key!, "default value");
        }
    }
}