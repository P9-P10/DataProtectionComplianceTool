using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Binders;

public class ProcessingBinder : BaseBinder<string, Processing>
{
    private readonly Option<string> _purposeOption;
    private readonly Option<TableColumnPair> _tableColumnOption;
    private readonly IManager<string, Purpose> _purposesManager;
    private readonly IManager<TableColumnPair, PersonalDataColumn> _personalDataColumnManager;
    private readonly ILineReader _reader;

    public ProcessingBinder(
        Option<string> keyOption,
        Option<string> descriptionOption,
        Option<string> purposeOption,
        Option<TableColumnPair> tableColumnOption,
        IManager<string, Purpose> purposesManager,
        IManager<TableColumnPair, PersonalDataColumn> personalDataColumnManager, ILineReader reader) : base(keyOption,
        descriptionOption)
    {
        _purposeOption = purposeOption;
        _tableColumnOption = tableColumnOption;
        _purposesManager = purposesManager;
        _personalDataColumnManager = personalDataColumnManager;
        _reader = reader;
    }

    protected override Processing GetBoundValue(BindingContext bindingContext)
    {
        var processing = base.GetBoundValue(bindingContext);

        if (bindingContext.ParseResult.HasOption(_purposeOption))
        {
            var purpose = bindingContext.ParseResult.GetValueForOption(_purposeOption);
            processing.Purpose = purpose is null
                ? null
                : Handlers.HandleMustExistWithCreateOnDemand(purpose, _purposesManager, _reader);
        }

        if (bindingContext.ParseResult.HasOption(_tableColumnOption))
        {
            var tableColumn = bindingContext.ParseResult.GetValueForOption(_tableColumnOption);
            processing.PersonalDataColumn = tableColumn is null
                ? null
                : Handlers.HandleMustExist(tableColumn, _personalDataColumnManager);
        }

        return processing;
    }
}