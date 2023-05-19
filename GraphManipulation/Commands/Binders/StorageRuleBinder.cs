using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Binders;

public class StorageRuleBinder : BaseBinder<string, StorageRule>
{
    private readonly IManager<TableColumnPair, PersonalDataColumn> _personalDataColumnManager;
    private readonly Option<TableColumnPair> _tableColumnOption;
    private readonly Option<string> _vacuumingConditionOption;

    public StorageRuleBinder(
        Option<string> keyOption,
        Option<string> descriptionOption,
        Option<string> vacuumingConditionOption,
        Option<TableColumnPair> tableColumnOption,
        IManager<TableColumnPair, PersonalDataColumn> personalDataColumnManager) : base(keyOption, descriptionOption)
    {
        _tableColumnOption = tableColumnOption;
        _personalDataColumnManager = personalDataColumnManager;
        _vacuumingConditionOption = vacuumingConditionOption;
    }

    protected override StorageRule GetBoundValue(BindingContext bindingContext)
    {
        var storageRule = base.GetBoundValue(bindingContext);

        if (bindingContext.ParseResult.HasOption(_tableColumnOption))
        {
            var tableColumn = bindingContext.ParseResult.GetValueForOption(_tableColumnOption);
            storageRule.PersonalDataColumn = tableColumn is null
                ? null
                : Handlers.HandleMustExistWithCreateOnDemand(tableColumn, _personalDataColumnManager);
        }

        if (bindingContext.ParseResult.HasOption(_vacuumingConditionOption))
        {
            storageRule.VacuumingCondition = bindingContext.ParseResult.GetValueForOption(_vacuumingConditionOption);
        }

        return storageRule;
    }
}