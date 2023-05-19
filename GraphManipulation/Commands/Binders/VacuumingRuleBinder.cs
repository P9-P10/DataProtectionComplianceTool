using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Binders;

public class VacuumingRuleBinder : BaseBinder<string, VacuumingRule>
{
    private readonly Option<string> _intervalOption;
    private readonly Option<IEnumerable<string>> _purposesOption;
    private readonly IManager<string, Purpose> _purposesManager;
    private readonly ILineReader _reader;

    public VacuumingRuleBinder(
        Option<string> keyOption,
        Option<string> descriptionOption,
        Option<string> intervalOption,
        Option<IEnumerable<string>> purposesOption,
        IManager<string, Purpose> purposesManager, ILineReader reader) : base(keyOption, descriptionOption)
    {
        _intervalOption = intervalOption;
        _purposesOption = purposesOption;
        _purposesManager = purposesManager;
        _reader = reader;
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
            rule.Purposes = purposes is null
                ? null
                : Handlers.HandleMustExistListWithCreateOnDemand(purposes, _purposesManager, _reader);
        }

        return rule;
    }
}