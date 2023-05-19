using System.CommandLine;
using GraphManipulation.Commands.Binders;
using GraphManipulation.Factories;
using GraphManipulation.Factories.Interfaces;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Builders;

public class StorageRuleCommandBuilder : BaseCommandBuilder<string, StorageRule>
{
    private readonly IManager<TableColumnPair, PersonalDataColumn> _personalDataColumnManager;

    public StorageRuleCommandBuilder(ICommandHandlerFactory commandHandlerFactory, IManagerFactory managerFactory) : base(commandHandlerFactory)
    {
        _personalDataColumnManager = managerFactory.CreateManager<TableColumnPair, PersonalDataColumn>();
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.StorageRulesName, CommandNamer.StorageRulesAlias,
            out var keyOption);

        var descriptionOption = OptionBuilder.CreateEntityDescriptionOption<StorageRule>();
        var newKeyOption = OptionBuilder.CreateNewNameOption<StorageRule>();

        var vacuumingConditionOption = BuildVacuumingConditionOption()
            .WithDescription("The condition that must be fulfilled for data to be deleted");

        var tableColumnOption = OptionBuilder
            .CreateTableColumnPairOption()
            .WithDescription("The data that will be vacuumed under this condition");

        var createBinder = new StorageRuleBinder(
            keyOption,
            descriptionOption,
            vacuumingConditionOption,
            tableColumnOption,
            _personalDataColumnManager
        );

        var updateBinder = new StorageRuleBinder(
            newKeyOption,
            descriptionOption,
            vacuumingConditionOption,
            tableColumnOption,
            _personalDataColumnManager
        );

        return baseCommand
            .WithSubCommands(
                CreateCommand(keyOption, createBinder, new Option[]
                {
                    descriptionOption, vacuumingConditionOption, tableColumnOption
                }),
                UpdateCommand(keyOption, updateBinder, new Option[]
                {
                    newKeyOption, descriptionOption, vacuumingConditionOption, tableColumnOption
                })
            );
    }

    protected override void StatusReport(StorageRule condition)
    {
        if (condition.VacuumingCondition is null)
        {
            FeedbackEmitter.EmitMissing(condition.Key!, "vacuuming condition");
        }

        if (condition.PersonalDataColumn is null)
        {
            FeedbackEmitter.EmitMissing<PersonalDataColumn>(condition.Key!);
        }
    }

    protected override Option<string> BuildKeyOption()
    {
        return OptionBuilder.CreateKeyOption<string, StorageRule>(OptionNamer.Name, OptionNamer.NameAlias);
    }

    private static Option<string> BuildVacuumingConditionOption()
    {
        return OptionBuilder
            .CreateOption<string>(OptionNamer.VacuumingCondition)
            .WithAlias(OptionNamer.VacuumingConditionAlias);
    }
}