using System.CommandLine;
using GraphManipulation.Commands.Builders.Binders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Vacuuming;

namespace GraphManipulation.Commands.Builders;

public class VacuumingRulesCommandBuilder : BaseCommandBuilder<string, VacuumingRule>
{
    private readonly IManager<string, Purpose> _purposesManager;
    private readonly IVacuumer _vacuumer;
    private readonly IManager<string, VacuumingRule> _vacuumingRulesManager;

    public VacuumingRulesCommandBuilder(IHandlerFactory handlerFactory, IManagerFactory managerFactory,
        IVacuumer vacuumer) : base(handlerFactory)
    {
        _purposesManager = managerFactory.CreateManager<string, Purpose>();
        _vacuumer = vacuumer;
        _vacuumingRulesManager = managerFactory.CreateManager<string, VacuumingRule>();
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.VacuumingRulesName, CommandNamer.VacuumingRulesAlias,
            out var keyOption);
        
        var descriptionOption = OptionBuilder.CreateEntityDescriptionOption<VacuumingRule>();
        var newKeyOption = OptionBuilder.CreateNewNameOption<VacuumingRule>();

        var intervalOption = BuildIntervalOption();

        var purposeListOption = OptionBuilder
            .CreatePurposeListOption()
            .WithDescription("The purpose(s) under which the personal data is stored");

        var purposeListChangesCommands = BuildListChangesCommand(
            keyOption, purposeListOption, _purposesManager,
            column => column.Purposes ?? new List<Purpose>(),
            (column, purposes) => column.Purposes = purposes);

        var createBinder = new VacuumingRuleBinder(
            keyOption,
            descriptionOption,
            intervalOption,
            purposeListOption,
            _purposesManager
        );
        
        var updateBinder = new VacuumingRuleBinder(
            newKeyOption,
            descriptionOption,
            intervalOption,
            purposeListOption,
            _purposesManager
        );

        return baseCommand
            .WithSubCommands(
                CreateCommand(keyOption, createBinder, new Option[]
                {
                    descriptionOption, intervalOption, purposeListOption
                }).WithValidator(result => OptionBuilder.ValidInterval(result, intervalOption)),
                UpdateCommand(keyOption, updateBinder, new Option[]
                {
                    newKeyOption, descriptionOption, intervalOption, purposeListOption
                }).WithValidator(result => OptionBuilder.ValidInterval(result, intervalOption)),
                ExecuteCommand(),
                purposeListChangesCommands.Add,
                purposeListChangesCommands.Remove
            );
    }

    protected override void StatusReport(VacuumingRule rule)
    {
        if (rule.Interval is null)
        {
            Emitter.EmitMissing(rule.Key!, "interval");
        }
    }

    protected override Option<string> BuildKeyOption()
    {
        return OptionBuilder.CreateKeyOption<string, VacuumingRule>(OptionNamer.Name, OptionNamer.NameAlias);
    }

    private Command ExecuteCommand()
    {
        return CommandBuilder
            .CreateNewCommand(CommandNamer.Execute)
            .WithAlias(CommandNamer.ExecuteAlias)
            .WithDescription("Executes the given vacuuming rule(s)")
            .WithOption(out var rulesOption,
                OptionBuilder
                    .CreateOption<IEnumerable<string>>(OptionNamer.Rules)
                    .WithAlias(OptionNamer.RulesAlias)
                    .WithDescription("The name(s) of the vacuuming rule(s) that should be executed")
                    .WithIsRequired(true)
                    .WithArity(ArgumentArity.OneOrMore)
                    .WithAllowMultipleArguments(true)
                    .WithDefaultValue(new List<string>()))
            .WithHandler(context =>
            {
                var ruleNames = context.ParseResult.GetValueForOption(rulesOption)!.ToList();
                var rules = new List<VacuumingRule>();

                foreach (var ruleName in ruleNames)
                {
                    var rule =  _vacuumingRulesManager.Get(ruleName);
                    if (rule is null)
                    {
                        Emitter.EmitCouldNotFind(ruleName);
                        return;
                    }

                    rules.Add(rule);
                }

                foreach (var rule in rules)
                {
                    _vacuumer.ExecuteVacuumingRules(new[] { rule });
                    Emitter.EmitSuccess(rule.Key!, FeedbackEmitter<string, VacuumingRule>.Operations.Executed, rule);
                }
            });
    }

    private static Option<string> BuildIntervalOption()
    {
        return OptionBuilder
            .CreateOption<string>(OptionNamer.Interval)
            .WithAlias(OptionNamer.IntervalAlias)
            .WithDescription("The interval in which the vacuuming rule should be executed");
    }
}