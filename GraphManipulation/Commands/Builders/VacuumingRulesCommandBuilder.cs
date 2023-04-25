using System.CommandLine;
using System.Data.Entity.Infrastructure.Design;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class VacuumingRulesCommandBuilder
{
    public static Command Build(IConsole console, IVacuumingRulesManager vacuumingRulesManager, IPurposesManager purposesManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.VacuumingRulesName)
            .WithAlias(CommandNamer.VacuumingRulesAlias)
            .WithSubCommands(
                Add(console, vacuumingRulesManager, purposesManager),
                Update(console, vacuumingRulesManager),
                Delete(console, vacuumingRulesManager),
                List(console, vacuumingRulesManager),
                Show(console, vacuumingRulesManager)
            );
    }

    private static Command Add(IConsole console, IVacuumingRulesManager vacuumingRulesManager, IPurposesManager purposesManager)
    {
        return CommandBuilder
            .BuildAddCommand()
            .WithDescription("Adds a vacuuming rule to the system")
            .WithOption(out var nameOption, BuildNameOption())
            .WithOption(out var intervalOption, BuildIntervalOption().WithIsRequired(true))
            .WithOption(out var purposeOption, 
                OptionBuilder
                    .CreateOption<string>("--purpose")
                    .WithAlias("-p")
                    .WithDescription("The purpose whose delete conditions should be executed by the rule")
                    .WithIsRequired(true))
            .WithOption(out var descriptionOption,
                OptionBuilder
                    .CreateDescriptionOption<string>()
                    .WithDescription("The description of the vacuuming rule")
                    .WithGetDefaultValue(() => ""))
            .WithHandler(context =>
            {
                Handlers.AddHandlerKey(context, console, 
                    (name, purpose, interval) => vacuumingRulesManager.AddVacuumingRule(name, interval, purpose),
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
            })
            ;
    }
    
    private static Command Update(IConsole console, IVacuumingRulesManager vacuumingRulesManager)
    {
        return CommandBuilder.BuildUpdateCommand();
    }
    
    private static Command Delete(IConsole console, IVacuumingRulesManager vacuumingRulesManager)
    {
        return CommandBuilder.BuildDeleteCommand();
    }
    
    private static Command List(IConsole console, IVacuumingRulesManager vacuumingRulesManager)
    {
        return CommandBuilder.BuildListCommand();
    }
    
    private static Command Show(IConsole console, IVacuumingRulesManager vacuumingRulesManager)
    {
        return CommandBuilder.BuildShowCommand();
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
            .CreateOption<string>("--interval")
            .WithAlias("-i")
            .WithDescription("The interval in which the vacuuming rule should be executed");
    }
}