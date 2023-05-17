using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using GraphManipulation.Managers;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Binders;

public class DeleteConditionBinder : BaseBinder<string, StorageRule>
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

    protected override StorageRule GetBoundValue(BindingContext bindingContext)
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
            deleteCondition.VacuumingCondition = bindingContext.ParseResult.GetValueForOption(_conditionOption);
        }

        return deleteCondition;
    }
}