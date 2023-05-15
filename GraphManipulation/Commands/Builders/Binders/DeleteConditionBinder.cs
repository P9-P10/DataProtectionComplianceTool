using System.CommandLine;
using System.CommandLine.Binding;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders.Binders;

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

        var tableColumn = bindingContext.ParseResult.GetValueForOption(_tableColumnOption)!;
        deleteCondition.PersonalDataColumn = HandleMustExistWithCreateOnDemand(tableColumn, _personalDataColumnManager);

        deleteCondition.Condition = bindingContext.ParseResult.GetValueForOption(_conditionOption);

        return deleteCondition;
    }
}