using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Managers.Interfaces.Archive;

namespace GraphManipulation.Commands.Builders;

public static class VacuumingRulesCommandBuilder
{
    public static Command Build(IConsole console, IVacuumingRulesManager vacuumingRulesManager,
        IPurposesManager purposesManager)
    {
        return CommandBuilder.CreateNewCommand(CommandNamer.VacuumingRulesName)
            .WithAlias(CommandNamer.VacuumingRulesAlias)
            .WithSubCommands(
                Add(console, vacuumingRulesManager, purposesManager),
                Update(console, vacuumingRulesManager),
                Delete(console, vacuumingRulesManager),
                List(console, vacuumingRulesManager),
                Show(console, vacuumingRulesManager),
                Execute(console, vacuumingRulesManager),
                AddPurpose(console, vacuumingRulesManager, purposesManager),
                RemovePurpose(console, vacuumingRulesManager)
            );
    }

    private static Command Add(IConsole console, IVacuumingRulesManager vacuumingRulesManager,
        IPurposesManager purposesManager)
    {
        return CommandBuilder
            .BuildCreateCommand()
            .WithDescription("Adds a vacuuming rule to the system")
            .WithOption(out var nameOption, BuildNameOption())
            .WithOption(out var intervalOption, BuildIntervalOption().WithIsRequired(true))
            .WithValidator(result => OptionBuilder.ValidInterval(result, intervalOption))
            .WithOption(out var purposeOption,
                OptionBuilder
                    .CreateOption<string>(OptionNamer.Purpose)
                    .WithAlias(OptionNamer.PurposeAlias)
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

                Handlers.UpdateHandlerUnique(context, console,
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
                    .WithAllowMultipleArguments(true))
            .WithHandler(context => Handlers.ExecuteHandlerList(context, console,
                vacuumingRulesManager.ExecuteRule,
                vacuumingRulesManager,
                rulesOption));
    }

    private static Command AddPurpose(IConsole console, IVacuumingRulesManager vacuumingRulesManager,
        IPurposesManager purposesManager)
    {
        return CommandBuilder
            .BuildCreateCommand("purpose")
            .WithDescription("Adds the given purpose(s) to the vacuuming rule")
            .WithOption(out var nameOption, BuildNameOption())
            .WithOption(out var purposeOption, BuildPurposeListOption())
            .WithHandler(context => Handlers.UpdateHandlerWithKeyList(context, console,
                vacuumingRulesManager.AddPurpose,
                vacuumingRulesManager,
                purposesManager,
                column => column.GetPurposes().Select(p => p.GetName()),
                nameOption,
                purposeOption
            ));
    }

    private static Command RemovePurpose(IConsole console, IVacuumingRulesManager vacuumingRulesManager)
    {
        return CommandBuilder
            .BuildRemoveCommand("purpose")
            .WithDescription("Removes the given purpose(s) from the vacuuming rule")
            .WithOption(out var nameOption, BuildNameOption())
            .WithOption(out var purposeOption, BuildPurposeListOption())
            .WithHandler(context => Handlers.RemoveHandlerKeyList(context, console,
                vacuumingRulesManager.RemovePurpose,
                vacuumingRulesManager,
                column => column.GetPurposes().Select(p => p.GetName()),
                nameOption,
                purposeOption));
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
            .CreateOption<string>(OptionNamer.Interval)
            .WithAlias(OptionNamer.IntervalAlias)
            .WithDescription("The interval in which the vacuuming rule should be executed");
    }
    
    private static Option<IEnumerable<string>> BuildPurposeListOption()
    {
        return OptionBuilder
            .CreatePurposeListOption()
            .WithDescription("The purpose(s) under which the personal data is stored")
            .WithIsRequired(true);
    }
}