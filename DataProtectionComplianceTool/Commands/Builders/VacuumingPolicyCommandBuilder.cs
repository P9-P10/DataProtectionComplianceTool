using System.CommandLine;
using GraphManipulation.Commands.Binders;
using GraphManipulation.Factories.Interfaces;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;
using GraphManipulation.Vacuuming;

namespace GraphManipulation.Commands.Builders;

public class VacuumingPolicyCommandBuilder : BaseCommandBuilder<string, VacuumingPolicy>
{
    private readonly IManager<string, Purpose> _purposesManager;
    private readonly IVacuumer _vacuumer;
    private readonly IManager<string, VacuumingPolicy> _vacuumingPoliciesManager;

    public VacuumingPolicyCommandBuilder(ICommandHandlerFactory commandHandlerFactory, IManagerFactory managerFactory,
        IVacuumerFactory vacuumerFactory) : base(commandHandlerFactory)
    {
        _purposesManager = managerFactory.CreateManager<string, Purpose>();
        _vacuumer = vacuumerFactory.CreateVacuumer();
        _vacuumingPoliciesManager = managerFactory.CreateManager<string, VacuumingPolicy>();
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.VacuumingPolicyName, CommandNamer.VacuumingPolicyAlias,
            out var keyOption);

        var descriptionOption = OptionBuilder.CreateEntityDescriptionOption<VacuumingPolicy>();
        var newKeyOption = OptionBuilder.CreateNewNameOption<VacuumingPolicy>();

        var durationOption = BuildDurationOption();

        var purposeListOption = OptionBuilder
            .CreatePurposeListOption()
            .WithDescription("The purpose(s) under which the personal data is stored");

        var purposeListChangesCommands = BuildListChangesCommand(
            keyOption, purposeListOption, _purposesManager,
            column => column.Purposes ?? new List<Purpose>(),
            (column, purposes) => column.Purposes = purposes);

        var createBinder = new VacuumingPolicyBinder(
            keyOption,
            descriptionOption,
            durationOption,
            purposeListOption,
            _purposesManager
        );

        var updateBinder = new VacuumingPolicyBinder(
            newKeyOption,
            descriptionOption,
            durationOption,
            purposeListOption,
            _purposesManager
        );

        return baseCommand
            .WithSubCommands(
                CreateCommand(keyOption, createBinder, new Option[]
                {
                    descriptionOption, durationOption, purposeListOption
                }).WithValidator(result => OptionBuilder.ValidDuration(result, durationOption)),
                UpdateCommand(keyOption, updateBinder, new Option[]
                {
                    newKeyOption, descriptionOption, durationOption
                }).WithValidator(result => OptionBuilder.ValidDuration(result, durationOption)),
                ExecuteCommand(),
                purposeListChangesCommands.Add,
                purposeListChangesCommands.Remove
            );
    }

    protected override void StatusReport(VacuumingPolicy policy)
    {
        if (policy.Duration is null)
        {
            FeedbackEmitter.EmitMissing(policy.Key!, "duration");
        }
    }

    protected override Option<string> BuildKeyOption()
    {
        return OptionBuilder.CreateKeyOption<string, VacuumingPolicy>(OptionNamer.Name, OptionNamer.NameAlias);
    }

    private Command ExecuteCommand()
    {
        return CommandBuilder
            .CreateNewCommand(CommandNamer.Execute)
            .WithAlias(CommandNamer.ExecuteAlias)
            .WithDescription("Executes the given vacuuming policies")
            .WithOption(out var policiesOption,
                OptionBuilder
                    .CreateOption<IEnumerable<string>>(OptionNamer.VacuumingPolicyList)
                    .WithAlias(OptionNamer.VacuumingPolicyListAlias)
                    .WithDescription("The names of the vacuuming policies that should be executed")
                    .WithIsRequired(true)
                    .WithArity(ArgumentArity.OneOrMore)
                    .WithAllowMultipleArguments(true))
            .WithHandler(context =>
            {
                var vacuumingPolicyNames = context.ParseResult.GetValueForOption(policiesOption)!.ToList();
                var vacuumingPolicies = new List<VacuumingPolicy>();

                foreach (var vacuumingPolicyName in vacuumingPolicyNames)
                {
                    var vacuumingPolicy = _vacuumingPoliciesManager.Get(vacuumingPolicyName);
                    if (vacuumingPolicy is null)
                    {
                        FeedbackEmitter.EmitCouldNotFind(vacuumingPolicyName);
                        return;
                    }

                    vacuumingPolicies.Add(vacuumingPolicy);
                }

                foreach (var vacuumingPolicy in vacuumingPolicies)
                {
                    FeedbackEmitter.EmitMessage($"Executing {vacuumingPolicy.ToListingIdentifier()}...");
                    var executions = _vacuumer.ExecuteVacuumingPolicy(vacuumingPolicy);
                    if (!executions.Any())
                    {
                        FeedbackEmitter.EmitMessage($"{vacuumingPolicy.ToListingIdentifier()} had no effect");
                    }

                    FeedbackEmitter.EmitResult(vacuumingPolicy.Key!, SystemOperation.Operation.Executed);
                }
            });
    }

    private static Option<string> BuildDurationOption()
    {
        return OptionBuilder
            .CreateOption<string>(OptionNamer.Duration)
            .WithAlias(OptionNamer.DurationAlias)
            .WithDescription("The duration between vacuuming policy executions");
    }
}