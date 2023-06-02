using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Binders;

public class StoragePolicyBinder : BaseBinder<string, StoragePolicy>
{
    private readonly IManager<TableColumnPair, PersonalDataColumn> _personalDataColumnManager;
    private readonly Option<TableColumnPair> _tableColumnOption;
    private readonly Option<string> _vacuumingConditionOption;

    public StoragePolicyBinder(
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

    protected override StoragePolicy GetBoundValue(BindingContext bindingContext)
    {
        var storagePolicy = base.GetBoundValue(bindingContext);

        if (bindingContext.ParseResult.HasOption(_tableColumnOption))
        {
            var tableColumn = bindingContext.ParseResult.GetValueForOption(_tableColumnOption);
            storagePolicy.PersonalDataColumn = tableColumn is null
                ? null
                : Handlers.HandleMustExistWithCreateOnDemand(tableColumn, _personalDataColumnManager);
        }

        if (bindingContext.ParseResult.HasOption(_vacuumingConditionOption))
        {
            storagePolicy.VacuumingCondition = bindingContext.ParseResult.GetValueForOption(_vacuumingConditionOption);
        }

        return storagePolicy;
    }
}