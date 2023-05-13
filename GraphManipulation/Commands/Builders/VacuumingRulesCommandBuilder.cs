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
    
    public VacuumingRulesCommandBuilder(
        IManager<string, VacuumingRule> manager,
        IManager<string, Purpose> purposesManager, 
        IVacuumer vacuumer) : base(manager)
    {
        _purposesManager = purposesManager;
        _vacuumer = vacuumer;
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.VacuumingRulesName, CommandNamer.VacuumingRulesAlias,
            out var keyOption);
        
        var descriptionOption = BuildDescriptionOption();
        var newKeyOption = BuildNewNameOption();

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
                }),
                UpdateCommand(keyOption, updateBinder, new Option[]
                {
                    newKeyOption, descriptionOption, intervalOption, purposeListOption
                }),
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
        return base.BuildKeyOption(OptionNamer.Name, OptionNamer.NameAlias, "The name of the vacuuming rule");
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
                    var rule = Manager.Get(ruleName);
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