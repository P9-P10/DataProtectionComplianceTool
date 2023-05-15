using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders.Binders;

public class PersonalDataOriginBinder : BaseBinder<int, PersonalDataOrigin>
{
    
    private readonly Option<int?> _individualOption;
    private readonly Option<TableColumnPair> _tableColumnOption;
    private readonly Option<string> _originOption;
    private readonly IManager<int, Individual> _individualsManager;
    private readonly IManager<TableColumnPair, PersonalDataColumn> _personalDataColumnManager;
    private readonly IManager<string, Origin> _originsManager;
    
    
    public PersonalDataOriginBinder(
        Option<int> keyOption, 
        Option<int?> individualOption,
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
            pdo.Individual = individual is null ? null : HandleMustExist(individual.Value, _individualsManager);
        }

        if (bindingContext.ParseResult.HasOption(_tableColumnOption))
        {
            var pdc = bindingContext.ParseResult.GetValueForOption(_tableColumnOption);
            pdo.PersonalDataColumn = pdc is null ? null : HandleMustExist(pdc, _personalDataColumnManager);
        }

        if (bindingContext.ParseResult.HasOption(_originOption))
        {
            var origin = bindingContext.ParseResult.GetValueForOption(_originOption);
            pdo.Origin = origin is null ? null : HandleMustExist(origin, _originsManager);
        }

        return pdo;
    }
}