using System.CommandLine;
using GraphManipulation.Commands.Builders.Binders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders;

public class DeleteConditionsCommandBuilder : BaseCommandBuilder<string, StorageRule>
{
    private readonly IManager<TableColumnPair, PersonalDataColumn> _personalDataColumnManager;

    public DeleteConditionsCommandBuilder(
        IManager<string, StorageRule> manager,
        IManager<TableColumnPair, PersonalDataColumn> personalDataColumnManager) : base(manager)
    {
        _personalDataColumnManager = personalDataColumnManager;
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.DeleteConditionsName, CommandNamer.DeleteConditionsAlias,
            out var keyOption);

        var descriptionOption = OptionBuilder.CreateEntityDescriptionOption<StorageRule>();
        var newKeyOption = OptionBuilder.CreateNewNameOption<StorageRule>();

        var conditionOption = BuildConditionOption()
            .WithDescription("The condition that must be fulfilled for data to be deleted");

        var tableColumnOption = OptionBuilder
            .CreateTableColumnPairOption()
            .WithDescription("The data that will be vacuumed under this condition");

        var createBinder = new DeleteConditionBinder(
            keyOption,
            descriptionOption,
            conditionOption,
            tableColumnOption,
            _personalDataColumnManager
        );

        var updateBinder = new DeleteConditionBinder(
            newKeyOption,
            descriptionOption,
            conditionOption,
            tableColumnOption,
            _personalDataColumnManager
        );

        return baseCommand
            .WithSubCommands(
                CreateCommand(keyOption, createBinder, new Option[]
                {
                    descriptionOption, conditionOption.WithIsRequired(true), tableColumnOption.WithIsRequired(true)
                }),
                UpdateCommand(keyOption, updateBinder, new Option[]
                {
                    newKeyOption, descriptionOption, conditionOption, tableColumnOption
                })
            );
    }

    protected override void StatusReport(StorageRule condition)
    {
        if (condition.Condition is null)
        {
            Emitter.EmitMissing(condition.Key!, "condition");
        }

        if (condition.PersonalDataColumn is null)
        {
            Emitter.EmitMissing<TableColumnPair, PersonalDataColumn>(condition.Key!);
        }
    }

    protected override Option<string> BuildKeyOption()
    {
        return OptionBuilder.CreateKeyOption<string, StorageRule>(OptionNamer.Name, OptionNamer.NameAlias);
    }

    private static Option<string> BuildConditionOption()
    {
        return OptionBuilder
            .CreateOption<string>(OptionNamer.Condition)
            .WithAlias(OptionNamer.ConditionAlias);
    }
}