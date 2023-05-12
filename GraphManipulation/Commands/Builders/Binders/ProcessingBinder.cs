using System.CommandLine;
using System.CommandLine.Binding;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
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

        var purpose = bindingContext.ParseResult.GetValueForOption(_purposeOption)!;
        processing.Purpose = HandleMustExist(purpose, _purposesManager);

        var tableColumn = bindingContext.ParseResult.GetValueForOption(_tableColumnOption)!;
        processing.PersonalDataColumn = HandleMustExist(tableColumn, _personalDataColumnManager);

        return processing;
    }
}