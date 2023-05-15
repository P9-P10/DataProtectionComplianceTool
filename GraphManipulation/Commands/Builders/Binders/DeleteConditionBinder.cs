using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders.Binders;

public class DeleteConditionBinder : BaseBinder<string, DeleteCondition>
{
    private readonly Option<string> _conditionOption;
    private readonly Option<TableColumnPair> _tableColumnOption;
    private readonly IManager<TableColumnPair, PersonalDataColumn> _personalDataColumnManager;

    public DeleteConditionBinder(
        Option<string> keyOption,
        Option<string> descriptionOption,
        Option<string> conditionOption,
        Option<TableColumnPair> tableColumnOption,
        IManager<TableColumnPair, PersonalDataColumn> personalDataColumnManager) : base(keyOption, descriptionOption)
    {
        _tableColumnOption = tableColumnOption;
        _personalDataColumnManager = personalDataColumnManager;
        _conditionOption = conditionOption;
    }

    protected override DeleteCondition GetBoundValue(BindingContext bindingContext)
    {
        var deleteCondition = base.GetBoundValue(bindingContext);

        if (bindingContext.ParseResult.HasOption(_tableColumnOption))
        {
            var tableColumn = bindingContext.ParseResult.GetValueForOption(_tableColumnOption);
            deleteCondition.PersonalDataColumn = tableColumn is null
                ? null
                : HandleMustExistWithCreateOnDemand(tableColumn, _personalDataColumnManager);
        }

        if (bindingContext.ParseResult.HasOption(_conditionOption))
        {
            deleteCondition.Condition = bindingContext.ParseResult.GetValueForOption(_conditionOption);
        }

        return deleteCondition;
    }
}