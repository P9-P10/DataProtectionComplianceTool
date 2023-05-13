using System.CommandLine;
using System.CommandLine.Binding;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders.Binders;

public class PurposeBinder : BaseBinder<string, Purpose>
{
    private readonly Option<bool> _legallyRequiredOption;
    private readonly Option<IEnumerable<string>> _deleteConditionsOption;
    private readonly IManager<string, DeleteCondition> _deleteConditionsManager;

    public PurposeBinder(
        Option<string> keyOption, 
        Option<string> descriptionOption, 
        Option<bool> legallyRequiredOption, 
        Option<IEnumerable<string>> deleteConditionsOption,
        IManager<string, DeleteCondition> deleteConditionsManager) : base(keyOption, descriptionOption)
    {
        _legallyRequiredOption = legallyRequiredOption;
        _deleteConditionsOption = deleteConditionsOption;
        _deleteConditionsManager = deleteConditionsManager;
    }

    protected override Purpose GetBoundValue(BindingContext bindingContext)
    {
        var purpose = base.GetBoundValue(bindingContext);

        purpose.LegallyRequired = bindingContext.ParseResult.GetValueForOption(_legallyRequiredOption);
        
        var deleteConditionNames = bindingContext.ParseResult.GetValueForOption(_deleteConditionsOption)!;

        purpose.DeleteConditions = HandleMustExistList(deleteConditionNames, _deleteConditionsManager);

        return purpose;
    }
}