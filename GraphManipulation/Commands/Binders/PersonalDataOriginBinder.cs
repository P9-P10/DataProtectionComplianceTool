using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Binders;

public class PersonalDataOriginBinder : BaseBinder<int, PersonalDataOrigin>
{
    private readonly Option<int> _individualOption;
    private readonly IManager<int, Individual> _individualsManager;
    private readonly Option<string> _originOption;
    private readonly IManager<string, Origin> _originsManager;
    private readonly IManager<TableColumnPair, PersonalDataColumn> _personalDataColumnManager;
    private readonly Option<TableColumnPair> _tableColumnOption;


    public PersonalDataOriginBinder(
        Option<int> keyOption,
        Option<int> individualOption,
        Option<string> descriptionOption,
        Option<TableColumnPair> tableColumnOption,
        Option<string> originOption,
        IManager<int, Individual> individualsManager,
        IManager<TableColumnPair, PersonalDataColumn> personalDataColumnManager,
        IManager<string, Origin> originsManager) : base(keyOption, descriptionOption)
    {
        _individualOption = individualOption;
        _tableColumnOption = tableColumnOption;
        _originOption = originOption;
        _individualsManager = individualsManager;
        _personalDataColumnManager = personalDataColumnManager;
        _originsManager = originsManager;
    }

    protected override PersonalDataOrigin GetBoundValue(BindingContext bindingContext)
    {
        var pdo = base.GetBoundValue(bindingContext);

        if (bindingContext.ParseResult.HasOption(_individualOption))
        {
            var individual = bindingContext.ParseResult.GetValueForOption(_individualOption);
            pdo.Individual = Handlers.HandleMustExist(individual, _individualsManager);
        }

        if (bindingContext.ParseResult.HasOption(_tableColumnOption))
        {
            var pdc = bindingContext.ParseResult.GetValueForOption(_tableColumnOption);
            pdo.PersonalDataColumn = pdc is null ? null : Handlers.HandleMustExist(pdc, _personalDataColumnManager);
        }

        if (bindingContext.ParseResult.HasOption(_originOption))
        {
            var origin = bindingContext.ParseResult.GetValueForOption(_originOption);
            pdo.Origin = origin is null ? null : Handlers.HandleMustExistWithCreateOnDemand(origin, _originsManager);
        }

        return pdo;
    }
}