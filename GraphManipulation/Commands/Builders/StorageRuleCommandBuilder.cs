using System.CommandLine;
using GraphManipulation.Commands.Binders;
using GraphManipulation.Factories;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Builders;

public class StorageRuleCommandBuilder : BaseCommandBuilder<string, StorageRule>
{
    private readonly IManager<TableColumnPair, PersonalDataColumn> _personalDataColumnManager;

    public StorageRuleCommandBuilder(IHandlerFactory handlerFactory, IManagerFactory managerFactory) : base(handlerFactory)
    {
        _personalDataColumnManager = managerFactory.CreateManager<TableColumnPair, PersonalDataColumn>();
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.StorageRulesName, CommandNamer.StorageRulesAlias,
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
                    descriptionOption, conditionOption, tableColumnOption
                }),
                UpdateCommand(keyOption, updateBinder, new Option[]
                {
                    newKeyOption, descriptionOption, conditionOption, tableColumnOption
                })
            );
    }

    protected override void StatusReport(StorageRule condition)
    {
        if (condition.VacuumingCondition is null)
        {
            FeedbackEmitter.EmitMissing(condition.Key!, "condition");
        }

        if (condition.PersonalDataColumn is null)
        {
            FeedbackEmitter.EmitMissing<TableColumnPair, PersonalDataColumn>(condition.Key!);
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