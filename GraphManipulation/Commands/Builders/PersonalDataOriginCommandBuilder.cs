using System.CommandLine;
using GraphManipulation.Commands.Builders.Binders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders;

public class PersonalDataOriginCommandBuilder : BaseCommandBuilder<int, PersonalDataOrigin>
{
    private readonly IManager<int, Individual> _individualsManager;
    private readonly IManager<TableColumnPair, PersonalDataColumn> _personalDataColumnManager;
    private readonly IManager<string, Origin> _originsManager;
    
    public PersonalDataOriginCommandBuilder(
        IManager<int, PersonalDataOrigin> manager, 
        IManager<int, Individual> individualsManager, 
        IManager<TableColumnPair, PersonalDataColumn> personalDataColumnManager, 
        IManager<string, Origin> originsManager) : base(manager)
    {
        _individualsManager = individualsManager;
        _personalDataColumnManager = personalDataColumnManager;
        _originsManager = originsManager;
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.PersonalDataOriginsName, CommandNamer.PersonalDataOriginsAlias,
            out var keyOption);
        
        var descriptionOption = OptionBuilder.CreateEntityDescriptionOption<PersonalDataOrigin>();

        var individualOption = OptionBuilder
            .CreateOption<int?>(OptionNamer.Individual)
            .WithAlias(OptionNamer.IndividualAlias)
            .WithDescription("The id of the individual, whose personal data is getting an origin");
        
        var tableColumnOption = OptionBuilder
            .CreateTableColumnPairOption()
            .WithDescription("The table and column that the personal data is stored in");

        var originOption = OptionBuilder
            .CreateOption<string>(OptionNamer.Origin)
            .WithAlias(OptionNamer.OriginAlias)
            .WithDescription("The origin of the personal data");

        var createBinder = new PersonalDataOriginBinder(
            keyOption,
            individualOption,
            descriptionOption,
            tableColumnOption,
            originOption,
            _individualsManager,
            _personalDataColumnManager, 
            _originsManager);

        var updateBinder = new PersonalDataOriginBinder(
            keyOption,
            individualOption,
            descriptionOption,
            tableColumnOption,
            originOption,
            _individualsManager,
            _personalDataColumnManager,
            _originsManager);

        return baseCommand
            .WithSubCommands(
                CreateCommand(keyOption, createBinder, new Option[]
                {
                    descriptionOption, individualOption, tableColumnOption, originOption
                }),
                UpdateCommand(keyOption, updateBinder, new Option[]
                {
                    descriptionOption, individualOption, tableColumnOption, originOption
                }));
    }

    protected override Option<int> BuildKeyOption()
    {
        return OptionBuilder.CreateKeyOption<int, PersonalDataOrigin>(OptionNamer.Id, OptionNamer.IdAlias, "id");
    }

    protected override void StatusReport(PersonalDataOrigin value)
    {
        // TODO: Report something?
    }
}