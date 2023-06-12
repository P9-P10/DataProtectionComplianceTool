using System.CommandLine;
using GraphManipulation.Commands.Binders;
using GraphManipulation.Factories.Interfaces;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Builders;

public class PersonalDataColumnCommandBuilder : BaseCommandBuilder<TableColumnPair, PersonalDataColumn>
{
    private readonly IManager<string, Purpose> _purposesManager;

    public PersonalDataColumnCommandBuilder(ICommandHandlerFactory commandHandlerFactory,
        IManagerFactory managerFactory) : base(commandHandlerFactory)
    {
        _purposesManager = managerFactory.CreateManager<string, Purpose>();
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.PersonalDataColumnName, CommandNamer.PersonalDataColumnAlias,
            out var keyOption);

        var descriptionOption = OptionBuilder.CreateEntityDescriptionOption<PersonalDataColumn>();

        var defaultValueOption = OptionBuilder
            .CreateOption<string>(OptionNamer.DefaultValue)
            .WithAlias(OptionNamer.DefaultValueAlias)
            .WithDescription("The default value that attributes in the column should receive upon deletion");
        
        var associationExpressionOption = OptionBuilder
            .CreateOption<string>(OptionNamer.AssociationExpression)
            .WithAlias(OptionNamer.AssociationExpressionAlias)
            .WithDescription("The expression that describes how this personal data column should be associated with the table containing individuals");

        var purposeListOption = OptionBuilder
            .CreatePurposeListOption()
            .WithDescription("The purpose(s) under which the personal data is stored");

        var createBinder = new PersonalDataColumnBinder(
            keyOption,
            descriptionOption,
            purposeListOption,
            defaultValueOption,
            associationExpressionOption,
            _purposesManager);

        var updateBinder = new PersonalDataColumnBinder(
            keyOption,
            descriptionOption,
            purposeListOption,
            defaultValueOption,
            associationExpressionOption,
            _purposesManager);

        var purposeListChangesCommands = BuildListChangesCommand(
            keyOption, purposeListOption, _purposesManager,
            column => column.Purposes ?? new List<Purpose>(),
            (column, purposes) => column.Purposes = purposes);

        return baseCommand
            .WithSubCommands(
                CreateCommand(keyOption, createBinder, new Option[]
                {
                    descriptionOption, defaultValueOption, associationExpressionOption, purposeListOption
                }),
                UpdateCommand(keyOption, updateBinder, new Option[]
                {
                    descriptionOption, defaultValueOption, associationExpressionOption
                }),
                purposeListChangesCommands.Add,
                purposeListChangesCommands.Remove
            );
    }

    protected override Option<TableColumnPair> BuildKeyOption()
    {
        return OptionBuilder.CreateTableColumnPairOption()
            .WithDescription("The table and column in which the personal data is stored")
            .WithIsRequired(true);
    }

    protected override void StatusReport(PersonalDataColumn column)
    {
        if (column.Purposes is null || !column.Purposes.Any())
        {
            FeedbackEmitter.EmitMissing<Purpose>(column.Key!);
        }

        if (column.DefaultValue is null)
        {
            FeedbackEmitter.EmitMissing(column.Key!, "default value");
        }

        if (column.AssociationExpression is null)
        {
            FeedbackEmitter.EmitMissing(column.Key!, "association expression");
        }
    }
}