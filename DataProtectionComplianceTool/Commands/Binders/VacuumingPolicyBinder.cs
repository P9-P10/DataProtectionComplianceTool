using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Binders;

public class VacuumingPolicyBinder : BaseBinder<string, VacuumingPolicy>
{
    private readonly Option<string> _durationOption;
    private readonly IManager<string, Purpose> _purposesManager;
    private readonly Option<IEnumerable<string>> _purposesOption;

    public VacuumingPolicyBinder(
        Option<string> keyOption,
        Option<string> descriptionOption,
        Option<string> durationOption,
        Option<IEnumerable<string>> purposesOption,
        IManager<string, Purpose> purposesManager) : base(keyOption, descriptionOption)
    {
        _durationOption = durationOption;
        _purposesOption = purposesOption;
        _purposesManager = purposesManager;
    }

    protected override VacuumingPolicy GetBoundValue(BindingContext bindingContext)
    {
        var vacuumingPolicy = base.GetBoundValue(bindingContext);

        if (bindingContext.ParseResult.HasOption(_durationOption))
        {
            vacuumingPolicy.Duration = bindingContext.ParseResult.GetValueForOption(_durationOption);
        }

        if (bindingContext.ParseResult.HasOption(_purposesOption))
        {
            var purposes = bindingContext.ParseResult.GetValueForOption(_purposesOption);
            vacuumingPolicy.Purposes = purposes is null
                ? null
                : Handlers.HandleMustExistListWithCreateOnDemand(purposes, _purposesManager);
        }

        return vacuumingPolicy;
    }
}