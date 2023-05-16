using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using GraphManipulation.Managers;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders.Binders;

public class VacuumingRuleBinder : BaseBinder<string, VacuumingRule>
{
    private readonly Option<string> _intervalOption;
    private readonly Option<IEnumerable<string>> _purposesOption;
    private readonly IManager<string, Purpose> _purposesManager;
    
    public VacuumingRuleBinder(
        Option<string> keyOption, 
        Option<string> descriptionOption, 
        Option<string> intervalOption, 
        Option<IEnumerable<string>> purposesOption, 
        IManager<string, Purpose> purposesManager) : base(keyOption, descriptionOption)
    {
        _intervalOption = intervalOption;
        _purposesOption = purposesOption;
        _purposesManager = purposesManager;
    }

    protected override VacuumingRule GetBoundValue(BindingContext bindingContext)
    {
        var rule = base.GetBoundValue(bindingContext);

        if (bindingContext.ParseResult.HasOption(_intervalOption))
        {
            rule.Interval = bindingContext.ParseResult.GetValueForOption(_intervalOption);
        }
        
        if (bindingContext.ParseResult.HasOption(_purposesOption))
        {
            var purposes = bindingContext.ParseResult.GetValueForOption(_purposesOption);
            rule.Purposes = purposes is null ? null : HandleMustExistListWithCreateOnDemand(purposes, _purposesManager);
        }

        return rule;
    }
}