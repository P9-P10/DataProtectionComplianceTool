using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Binders;

public class PurposeBinder : BaseBinder<string, Purpose>
{
    private readonly Option<bool> _legallyRequiredOption;
    private readonly Option<IEnumerable<string>> _storageRulesOption;
    private readonly IManager<string, StorageRule> _storageRulesManager;
    private readonly ILineReader _reader;

    public PurposeBinder(
        Option<string> keyOption,
        Option<string> descriptionOption,
        Option<bool> legallyRequiredOption,
        Option<IEnumerable<string>> storageRulesOption,
        IManager<string, StorageRule> storageRulesManager, ILineReader reader) : base(keyOption, descriptionOption)
    {
        _legallyRequiredOption = legallyRequiredOption;
        _storageRulesOption = storageRulesOption;
        _storageRulesManager = storageRulesManager;
        _reader = reader;
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
            purpose.StorageRules =
                Handlers.HandleMustExistListWithCreateOnDemand(storageRuleNames, _storageRulesManager, _reader);
        }

        return purpose;
    }
}