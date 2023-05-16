using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using GraphManipulation.Managers;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders.Binders;

public class ProcessingBinder : BaseBinder<string, Processing>
{
    private readonly Option<string> _purposeOption;
    private readonly Option<TableColumnPair> _tableColumnOption;
    private readonly IManager<string, Purpose> _purposesManager;
    private readonly IManager<TableColumnPair, PersonalDataColumn> _personalDataColumnManager;
    
    public ProcessingBinder(
        Option<string> keyOption, 
        Option<string> descriptionOption, 
        Option<string> purposeOption, 
        Option<TableColumnPair> tableColumnOption, 
        IManager<string, Purpose> purposesManager, 
        IManager<TableColumnPair, PersonalDataColumn> personalDataColumnManager) : base(keyOption, descriptionOption)
    {
        _purposeOption = purposeOption;
        _tableColumnOption = tableColumnOption;
        _purposesManager = purposesManager;
        _personalDataColumnManager = personalDataColumnManager;
    }

    protected override Processing GetBoundValue(BindingContext bindingContext)
    {
        var processing = base.GetBoundValue(bindingContext);

        if (bindingContext.ParseResult.HasOption(_purposeOption))
        {
            var purpose = bindingContext.ParseResult.GetValueForOption(_purposeOption);
            processing.Purpose = purpose is null ? null : HandleMustExistWithCreateOnDemand(purpose, _purposesManager);
        }

        if (bindingContext.ParseResult.HasOption(_tableColumnOption))
        {
            var tableColumn = bindingContext.ParseResult.GetValueForOption(_tableColumnOption);
            processing.PersonalDataColumn = tableColumn is null ? null : HandleMustExist(tableColumn, _personalDataColumnManager);
        }

        return processing;
    }
}