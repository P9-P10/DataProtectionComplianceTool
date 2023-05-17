using System.CommandLine;
using GraphManipulation.Commands.Binders;
using GraphManipulation.Factories;
using GraphManipulation.Factories.Interfaces;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Builders;

public class PersonalDataOriginCommandBuilder : BaseCommandBuilder<int, PersonalDataOrigin>
{
    private readonly IManager<int, Individual> _individualsManager;
    private readonly IManager<TableColumnPair, PersonalDataColumn> _personalDataColumnManager;
    private readonly IManager<string, Origin> _originsManager;
    
    public PersonalDataOriginCommandBuilder(ICommandHandlerFactory commandHandlerFactory, IManagerFactory managerFactory) : base(commandHandlerFactory)
    {
        _individualsManager = managerFactory.CreateManager<int, Individual>();
        _personalDataColumnManager = managerFactory.CreateManager<TableColumnPair, PersonalDataColumn>();
        _originsManager = managerFactory.CreateManager<string, Origin>();
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
                    individualOption, tableColumnOption, originOption
                }),
                UpdateCommand(keyOption, updateBinder, new Option[]
                {
                    individualOption, tableColumnOption, originOption
                }));
    }

    protected override Option<int> BuildKeyOption()
    {
        return OptionBuilder.CreateKeyOption<int, PersonalDataOrigin>(OptionNamer.Id, OptionNamer.IdAlias, "id");
    }

    protected override void StatusReport(PersonalDataOrigin value)
    {
        if (value.Individual is null)
        {
            FeedbackEmitter.EmitMissing<Individual>(value.Key);
        }
        // if (value.PersonalDataColumn is not null && value.Origin is null)
        // {
        //     FeedbackEmitter.EmitMissing(value.Key, $"{TypeToString.GetEntityType(typeof(Origin))} for {TypeToString.GetEntityType(typeof(Individual))} '{value.Individual!.Key}' " +
        //                                    $"and {TypeToString.GetEntityType(typeof(PersonalDataColumn))} '{value.PersonalDataColumn.Key}'");
        //     return;
        // } 
        
        if (value.PersonalDataColumn is null)
        {
            FeedbackEmitter.EmitMissing<PersonalDataColumn>(value.Key);
        }

        if (value.Origin is null)
        {
            FeedbackEmitter.EmitMissing<Origin>(value.Key);
        }
        
        
    }
}