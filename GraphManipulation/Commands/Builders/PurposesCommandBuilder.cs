using System.CommandLine;
using GraphManipulation.Commands.Builders.Binders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders;

public class PurposesCommandBuilder : BaseCommandBuilder<string, Purpose>
{
    private readonly IManager<string, DeleteCondition> _deleteConditionsManager;

    public PurposesCommandBuilder(
        IManager<string, Purpose> purposesManager,
        IManager<string, DeleteCondition> deleteConditionsManager) : base(purposesManager)
    {
        _deleteConditionsManager = deleteConditionsManager;
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.PurposesName, CommandNamer.PurposesAlias, out var keyOption);

        var descriptionOption = OptionBuilder.CreateEntityDescriptionOption<Purpose>();
        var newKeyOption = OptionBuilder.CreateNewNameOption<Purpose>();

        var legallyRequiredOption = BuildLegallyRequiredOption()
                .WithDescription("Whether the purpose falls under any legal obligations");

        var deleteConditionListOption = BuildDeleteConditionListOption()
                .WithDescription("The conditions for which personal data stored under this purpose should be deleted");

        var createBinder = new PurposeBinder(
            keyOption,
            descriptionOption,
            legallyRequiredOption,
            deleteConditionListOption,
            _deleteConditionsManager
        );

        var updateBinder = new PurposeBinder(
            newKeyOption,
            descriptionOption,
            legallyRequiredOption,
            deleteConditionListOption,
            _deleteConditionsManager
        );
        
        var deleteConditionsListChangesCommands = BuildListChangesCommand(
            keyOption, deleteConditionListOption, _deleteConditionsManager,
            purpose => purpose.DeleteConditions ?? new List<DeleteCondition>(),
            (purpose, deleteConditions) => purpose.DeleteConditions = deleteConditions);

        return baseCommand
            .WithSubCommands(
                CreateCommand(keyOption, createBinder, new Option[]
                {
                    descriptionOption, legallyRequiredOption, deleteConditionListOption
                }),
                UpdateCommand(keyOption, updateBinder, new Option[]
                {
                    newKeyOption, descriptionOption, legallyRequiredOption, deleteConditionListOption
                }),
                deleteConditionsListChangesCommands.Add,
                deleteConditionsListChangesCommands.Remove
            );
    }

    private static Option<bool> BuildLegallyRequiredOption()
    {
        return OptionBuilder
            .CreateOption<bool>(OptionNamer.LegallyRequired)
            .WithAlias(OptionNamer.LegallyRequiredAlias);
    }

    private static Option<IEnumerable<string>> BuildDeleteConditionListOption()
    {
        return OptionBuilder
            .CreateOption<IEnumerable<string>>(OptionNamer.DeleteConditionName)
            .WithAlias(OptionNamer.DeleteConditionNameAlias)
            .WithAllowMultipleArguments(true)
            .WithArity(ArgumentArity.OneOrMore);
    }


    protected override void StatusReport(Purpose purpose)
    {
        if (purpose.Rules is null || !purpose.Rules.Any())
        {
            Emitter.EmitMissing<string, VacuumingRule>(purpose.Key!);
        }

        if (purpose.DeleteConditions is null || !purpose.DeleteConditions.Any())
        {
            Emitter.EmitMissing<string, DeleteCondition>(purpose.Key!);
        }
    }

    protected override Option<string> BuildKeyOption()
    {
        return OptionBuilder.CreateKeyOption<string, Purpose>(OptionNamer.Name, OptionNamer.NameAlias);
    }
}