using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Binders;

public class PersonalDataColumnBinder : BaseBinder<TableColumnPair, PersonalDataColumn>
{
    private readonly Option<IEnumerable<string>> _purposesOption;
    private readonly Option<string> _defaultValueOption;
    private readonly IManager<string, Purpose> _purposesManager;

    public PersonalDataColumnBinder(
        Option<TableColumnPair> keyOption,
        Option<string> descriptionOption,
        Option<IEnumerable<string>> purposesOption,
        Option<string> defaultValueOption,
        IManager<string, Purpose> purposesManager) : base(keyOption, descriptionOption)
    {
        _purposesOption = purposesOption;
        _defaultValueOption = defaultValueOption;
        _purposesManager = purposesManager;
    }

    protected override PersonalDataColumn GetBoundValue(BindingContext bindingContext)
    {
        var pdc = base.GetBoundValue(bindingContext);

        if (bindingContext.ParseResult.HasOption(_defaultValueOption))
        {
            pdc.DefaultValue = bindingContext.ParseResult.GetValueForOption(_defaultValueOption);
        }

        if (bindingContext.ParseResult.HasOption(_purposesOption))
        {
            var purposes = bindingContext.ParseResult.GetValueForOption(_purposesOption);

            pdc.Purposes = purposes is null ? null : Handlers.HandleMustExistListWithCreateOnDemand(purposes, _purposesManager);
        }

        return pdc;
    }
}