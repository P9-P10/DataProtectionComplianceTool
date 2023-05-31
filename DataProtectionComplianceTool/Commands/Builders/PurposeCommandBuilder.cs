using System.CommandLine;
using GraphManipulation.Commands.Binders;
using GraphManipulation.Factories.Interfaces;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Builders;

public class PurposeCommandBuilder : BaseCommandBuilder<string, Purpose>
{
    private readonly IManager<string, StoragePolicy> _storagePolicyManager;

    public PurposeCommandBuilder(ICommandHandlerFactory commandHandlerFactory, IManagerFactory managerFactory) : base(
        commandHandlerFactory)
    {
        _storagePolicyManager = managerFactory.CreateManager<string, StoragePolicy>();
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.PurposeName, CommandNamer.PurposeAlias, out var keyOption);

        var descriptionOption = OptionBuilder.CreateEntityDescriptionOption<Purpose>();
        var newKeyOption = OptionBuilder.CreateNewNameOption<Purpose>();

        var legallyRequiredOption = BuildLegallyRequiredOption()
            .WithDescription("Whether the purpose falls under any legal obligations");

        var storagePolicyListOption = BuildStoragePolicyListOption()
            .WithDescription("The storage policies which personal data stored under this purpose should follow");

        var createBinder = new PurposeBinder(
            keyOption,
            descriptionOption,
            legallyRequiredOption,
            storagePolicyListOption,
            _storagePolicyManager
        );

        var updateBinder = new PurposeBinder(
            newKeyOption,
            descriptionOption,
            legallyRequiredOption,
            storagePolicyListOption,
            _storagePolicyManager
        );

        var storagePolicyListChangesCommands = BuildListChangesCommand(
            keyOption, storagePolicyListOption, _storagePolicyManager,
            purpose => purpose.StoragePolicies ?? new List<StoragePolicy>(),
            (purpose, storagePolicies) => purpose.StoragePolicies = storagePolicies);

        return baseCommand
            .WithSubCommands(
                CreateCommand(keyOption, createBinder, new Option[]
                {
                    descriptionOption, legallyRequiredOption, storagePolicyListOption
                }),
                UpdateCommand(keyOption, updateBinder, new Option[]
                {
                    newKeyOption, descriptionOption, legallyRequiredOption
                }),
                storagePolicyListChangesCommands.Add,
                storagePolicyListChangesCommands.Remove
            );
    }

    private static Option<bool> BuildLegallyRequiredOption()
    {
        return OptionBuilder
            .CreateOption<bool>(OptionNamer.LegallyRequired)
            .WithAlias(OptionNamer.LegallyRequiredAlias);
    }

    private static Option<IEnumerable<string>> BuildStoragePolicyListOption()
    {
        return OptionBuilder
            .CreateOption<IEnumerable<string>>(OptionNamer.StoragePolicyList)
            .WithAlias(OptionNamer.StoragePolicyListAlias)
            .WithAllowMultipleArguments(true)
            .WithArity(ArgumentArity.OneOrMore);
    }


    protected override void StatusReport(Purpose purpose)
    {
        if (purpose.LegallyRequired is null)
        {
            FeedbackEmitter.EmitMissing(purpose.Key!, "legally required value");
        }

        if (purpose.VacuumingPolicies is null || !purpose.VacuumingPolicies.Any())
        {
            FeedbackEmitter.EmitMissing<VacuumingPolicy>(purpose.Key!);
        }

        if (purpose.StoragePolicies is null || !purpose.StoragePolicies.Any())
        {
            FeedbackEmitter.EmitMissing<StoragePolicy>(purpose.Key!);
        }
    }

    protected override Option<string> BuildKeyOption()
    {
        return OptionBuilder.CreateKeyOption<string, Purpose>(OptionNamer.Name, OptionNamer.NameAlias);
    }
}