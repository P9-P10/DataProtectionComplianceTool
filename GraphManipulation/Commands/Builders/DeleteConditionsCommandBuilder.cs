using System.CommandLine;
using GraphManipulation.Commands.Builders.Binders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders;

public class DeleteConditionsCommandBuilder : BaseCommandBuilder<string, DeleteCondition>
{
    private readonly IManager<TableColumnPair, PersonalDataColumn> _personalDataColumnManager;

    public DeleteConditionsCommandBuilder(
        IConsole console,
        IManager<string, DeleteCondition> manager,
        IManager<TableColumnPair, PersonalDataColumn> personalDataColumnManager) : base(console, manager)
    {
        _personalDataColumnManager = personalDataColumnManager;
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.DeleteConditionsName, CommandNamer.DeleteConditionsAlias,
            out var keyOption);

        var descriptionOption = BuildDescriptionOption();
        var newKeyOption = BuildNewNameOption();

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

    protected override Option<string> BuildKeyOption()
    {
        return base.BuildKeyOption(OptionNamer.Name, OptionNamer.NameAlias, "The name of the delete condition");
    }

    private static Option<string> BuildConditionOption()
    {
        return OptionBuilder
            .CreateOption<string>(OptionNamer.Condition)
            .WithAlias(OptionNamer.ConditionAlias);
    }
}