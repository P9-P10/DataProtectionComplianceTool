using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Binders;

public class PersonalDataColumnBinder : BaseBinder<TableColumnPair, PersonalDataColumn>
{
    private readonly Option<string> _defaultValueOption;
    private readonly Option<string> _associationExpressionOption;
    private readonly IManager<string, Purpose> _purposesManager;
    private readonly IManager<string, LegalBasis> _legalBasesManager;
    private readonly Option<IEnumerable<string>> _purposesOption;
    private readonly Option<IEnumerable<string>> _legalBasesOption;

    public PersonalDataColumnBinder(
        Option<TableColumnPair> keyOption,
        Option<string> descriptionOption,
        Option<IEnumerable<string>> purposesOption,
        Option<IEnumerable<string>> legalBasesOption,
        Option<string> defaultValueOption,
        Option<string> associationExpressionOption,
        IManager<string, Purpose> purposesManager,
        IManager<string, LegalBasis> legalBasesManager) : base(keyOption, descriptionOption)
    {
        _purposesOption = purposesOption;
        _legalBasesOption = legalBasesOption;
        _defaultValueOption = defaultValueOption;
        _associationExpressionOption = associationExpressionOption;
        _purposesManager = purposesManager;
        _legalBasesManager = legalBasesManager;
    }

    protected override PersonalDataColumn GetBoundValue(BindingContext bindingContext)
    {
        var pdc = base.GetBoundValue(bindingContext);

        if (bindingContext.ParseResult.HasOption(_defaultValueOption))
        {
            pdc.DefaultValue = bindingContext.ParseResult.GetValueForOption(_defaultValueOption);
        }

        if (bindingContext.ParseResult.HasOption(_associationExpressionOption))
        {
            pdc.AssociationExpression = bindingContext.ParseResult.GetValueForOption(_associationExpressionOption);
        }

        if (bindingContext.ParseResult.HasOption(_purposesOption))
        {
            var purposes = bindingContext.ParseResult.GetValueForOption(_purposesOption);

            pdc.Purposes = purposes is null
                ? null
                : Handlers.HandleMustExistListWithCreateOnDemand(purposes, _purposesManager);
        }
        
        if (bindingContext.ParseResult.HasOption(_legalBasesOption))
        {
            var legalBases = bindingContext.ParseResult.GetValueForOption(_legalBasesOption);

            pdc.LegalBases = legalBases is null
                ? null
                : Handlers.HandleMustExistListWithCreateOnDemand(legalBases, _legalBasesManager);
        }

        return pdc;
    }
}