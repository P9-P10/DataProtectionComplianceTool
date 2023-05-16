using System.CommandLine;
using GraphManipulation.Commands.Binders;
using GraphManipulation.Factories;
using GraphManipulation.Factories.Interfaces;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Builders;

public class PurposesCommandBuilder : BaseCommandBuilder<string, Purpose>
{
    private readonly IManager<string, StorageRule> _deleteConditionsManager;

    public PurposesCommandBuilder(ICommandHandlerFactory commandHandlerFactory, IManagerFactory managerFactory) : base(commandHandlerFactory)
    {
        _deleteConditionsManager = managerFactory.CreateManager<string, StorageRule>();
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
            purpose => purpose.StorageRules ?? new List<StorageRule>(),
            (purpose, deleteConditions) => purpose.StorageRules = deleteConditions);

        return baseCommand
            .WithSubCommands(
                CreateCommand(keyOption, createBinder, new Option[]
                {
                    descriptionOption, legallyRequiredOption, deleteConditionListOption
                }),
                UpdateCommand(keyOption, updateBinder, new Option[]
                {
                    newKeyOption, descriptionOption, legallyRequiredOption
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
        if (purpose.LegallyRequired is null)
        {
            FeedbackEmitter.EmitMissing(purpose.Key!, "legally required value");
        }
        
        if (purpose.Rules is null || !purpose.Rules.Any())
        {
            FeedbackEmitter.EmitMissing<string, VacuumingRule>(purpose.Key!);
        }

        if (purpose.StorageRules is null || !purpose.StorageRules.Any())
        {
            FeedbackEmitter.EmitMissing<string, StorageRule>(purpose.Key!);
        }
    }

    protected override Option<string> BuildKeyOption()
    {
        return OptionBuilder.CreateKeyOption<string, Purpose>(OptionNamer.Name, OptionNamer.NameAlias);
    }
}