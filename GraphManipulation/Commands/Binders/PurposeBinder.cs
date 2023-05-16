using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using GraphManipulation.Managers;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Binders;

public class PurposeBinder : BaseBinder<string, Purpose>
{
    private readonly Option<bool> _legallyRequiredOption;
    private readonly Option<IEnumerable<string>> _storageRulesOption;
    private readonly IManager<string, StorageRule> _storageRulesManager;

    public PurposeBinder(
        Option<string> keyOption,
        Option<string> descriptionOption,
        Option<bool> legallyRequiredOption,
        Option<IEnumerable<string>> storageRulesOption,
        IManager<string, StorageRule> storageRulesManager) : base(keyOption, descriptionOption)
    {
        _legallyRequiredOption = legallyRequiredOption;
        _storageRulesOption = storageRulesOption;
        _storageRulesManager = storageRulesManager;
    }

    protected override Purpose GetBoundValue(BindingContext bindingContext)
    {
        var purpose = base.GetBoundValue(bindingContext);

        if (bindingContext.ParseResult.HasOption(_legallyRequiredOption))
        {
            purpose.LegallyRequired = bindingContext.ParseResult.GetValueForOption(_legallyRequiredOption);
        }

        if (bindingContext.ParseResult.HasOption(_storageRulesOption))
        {
            var storageRuleNames = bindingContext.ParseResult.GetValueForOption(_storageRulesOption)!;
            purpose.StorageRules = HandleMustExistListWithCreateOnDemand(storageRuleNames, _storageRulesManager);
        }

        return purpose;
    }
}