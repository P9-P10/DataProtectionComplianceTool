using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Binders;

public class PurposeBinder : BaseBinder<string, Purpose>
{
    private readonly IManager<string, StoragePolicy> _storagePoliciesManager;
    private readonly Option<IEnumerable<string>> _storagePoliciesOption;

    public PurposeBinder(
        Option<string> keyOption,
        Option<string> descriptionOption,
        Option<IEnumerable<string>> storagePoliciesOption,
        IManager<string, StoragePolicy> storagePoliciesManager) : base(keyOption, descriptionOption)
    {
        _storagePoliciesOption = storagePoliciesOption;
        _storagePoliciesManager = storagePoliciesManager;
    }

    protected override Purpose GetBoundValue(BindingContext bindingContext)
    {
        var purpose = base.GetBoundValue(bindingContext);
        
        if (bindingContext.ParseResult.HasOption(_storagePoliciesOption))
        {
            var storagePolicyNames = bindingContext.ParseResult.GetValueForOption(_storagePoliciesOption)!;
            purpose.StoragePolicies =
                Handlers.HandleMustExistListWithCreateOnDemand(storagePolicyNames, _storagePoliciesManager);
        }

        return purpose;
    }
}