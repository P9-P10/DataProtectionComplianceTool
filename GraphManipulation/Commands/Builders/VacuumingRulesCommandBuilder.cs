using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class VacuumingRulesCommandBuilder
{
    public static Command Build(IConsole console, IVacuumingRulesManager vacuumingRulesManager,
        IPurposesManager purposesManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.VacuumingRulesName)
            .WithAlias(CommandNamer.VacuumingRulesAlias)
            .WithSubCommands(
                Add(console, vacuumingRulesManager, purposesManager),
                Update(console, vacuumingRulesManager),
                Delete(console, vacuumingRulesManager),
                List(console, vacuumingRulesManager),
                Show(console, vacuumingRulesManager),
                Execute(console, vacuumingRulesManager)
            );
    }

    private static Command Add(IConsole console, IVacuumingRulesManager vacuumingRulesManager,
        IPurposesManager purposesManager)
    {
        return CommandBuilder
            .BuildAddCommand()
            .WithDescription("Adds a vacuuming rule to the system")
            .WithOption(out var nameOption, BuildNameOption())
            .WithOption(out var intervalOption, BuildIntervalOption().WithIsRequired(true))
            .WithOption(out var purposeOption,
                OptionBuilder
                    .CreateOption<string>(CommandNamer.PurposeOption)
                    .WithAlias(CommandNamer.PurposeOptionAlias)
                    .WithDescription("The purpose whose delete conditions should be executed by the rule")
                    .WithIsRequired(true))
            .WithOption(out var descriptionOption,
                OptionBuilder
                    .CreateDescriptionOption()
                    .WithDescription("The description of the vacuuming rule")
                    .WithGetDefaultValue(() => ""))
            .WithHandler(context =>
            {
                Handlers.AddHandlerKey(context, console,
                    (name, purpose, interval) =>
                        vacuumingRulesManager.AddVacuumingRule(name, interval, purpose),
                    vacuumingRulesManager,
                    purposesManager,
                    nameOption,
                    purposeOption,
                    intervalOption);

                Handlers.UpdateHandler(context, console,
                    vacuumingRulesManager.UpdateDescription,
                    vacuumingRulesManager,
                    rule => rule.GetDescription(),
                    nameOption,
                    descriptionOption);
            });
    }

    private static Command Update(IConsole console, IVacuumingRulesManager vacuumingRulesManager)
    {
        return CommandBuilder
            .BuildUpdateCommand()
            .WithDescription("Updates the given vacuuming rule with the given values")
            .WithOption(out var nameOption, BuildNameOption())
            .WithOption(out var newNameOption,
                OptionBuilder
                    .CreateNewNameOption()
                    .WithDescription("The new name of the vacuuming rule"))
            .WithOption(out var intervalOption, BuildIntervalOption())
            .WithOption(out var descriptionOption,
                OptionBuilder
                    .CreateDescriptionOption()
                    .WithDescription("The description of the vacuuming rule"))
            .WithHandler(context =>
            {
                Handlers.UpdateHandler(context, console,
                    vacuumingRulesManager.UpdateDescription,
                    vacuumingRulesManager,
                    rule => rule.GetDescription(),
                    nameOption,
                    descriptionOption);

                Handlers.UpdateHandler(context, console,
                    vacuumingRulesManager.UpdateInterval,
                    vacuumingRulesManager,
                    rule => rule.GetInterval(),
                    nameOption,
                    intervalOption);

                Handlers.UpdateHandler(context, console,
                    vacuumingRulesManager.UpdateName,
                    vacuumingRulesManager,
                    rule => rule.GetName(),
                    nameOption,
                    newNameOption);
            });
    }

    private static Command Delete(IConsole console, IVacuumingRulesManager vacuumingRulesManager)
    {
        return CommandBuilder
            .BuildDeleteCommand()
            .WithDescription("Deletes the given vacuuming rule from the system")
            .WithOption(out var nameOption, BuildNameOption())
            .WithHandler(context => Handlers.DeleteHandler(context, console,
                vacuumingRulesManager.Delete,
                vacuumingRulesManager,
                nameOption));
    }

    private static Command List(IConsole console, IVacuumingRulesManager vacuumingRulesManager)
    {
        return CommandBuilder
            .BuildListCommand()
            .WithDescription("Lists the vacuuming rules currently in the system")
            .WithHandler(() => Handlers.ListHandler(console, vacuumingRulesManager,CommandHeader.VacuumingHeader));
    }

    private static Command Show(IConsole console, IVacuumingRulesManager vacuumingRulesManager)
    {
        return CommandBuilder
            .BuildShowCommand()
            .WithDescription("Shows information about the given vacuuming rule")
            .WithOption(out var nameOption, BuildNameOption())
            .WithHandler(context => Handlers.ShowHandler(context, console, vacuumingRulesManager, nameOption));
    }

    private static Command Execute(IConsole console, IVacuumingRulesManager vacuumingRulesManager)
    {
        return CommandBuilder
            .CreateCommand(CommandNamer.ExecuteCommand)
            .WithAlias(CommandNamer.ExecuteAlias)
            .WithDescription("Executes the given vacuuming rule(s)")
            .WithOption(out var rulesOption,
                OptionBuilder
                    .CreateOption<IEnumerable<string>>(CommandNamer.RulesOption)
                    .WithAlias(CommandNamer.RulesOptionAlias)
                    .WithDescription("The name(s) of the vacuuming rule(s) that should be executed")
                    .WithIsRequired(true)
                    .WithArity(ArgumentArity.OneOrMore)
                    .WithAllowMultipleArguments(true))
            .WithHandler(context => Handlers.ExecuteHandlerList(context, console,
                vacuumingRulesManager.ExecuteRule,
                vacuumingRulesManager,
                rulesOption));
    }

    private static Option<string> BuildNameOption()
    {
        return OptionBuilder
            .CreateNameOption()
            .WithDescription("The name of the vacuuming rule")
            .WithIsRequired(true);
    }

    private static Option<string> BuildIntervalOption()
    {
        return OptionBuilder
            .CreateOption<string>(CommandNamer.IntervalOption)
            .WithAlias(CommandNamer.IntervalOptionAlias)
            .WithDescription("The interval in which the vacuuming rule should be executed");
    }
}