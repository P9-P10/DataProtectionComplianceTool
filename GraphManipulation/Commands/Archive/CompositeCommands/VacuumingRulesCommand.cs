using System.CommandLine;
using System.CommandLine.Parsing;
using GraphManipulation.Commands.BaseCommands;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Models;
using GraphManipulation.Vacuuming;

namespace GraphManipulation.Commands.CompositeCommands;

public class VacuumingRulesCommand : AliasedCommand
{
    public VacuumingRulesCommand(IVacuumer vacuumer, string? description = null)
        : base("vacuuming-rules", "vr", description)
    {
        // AddCommand(new AddVacuumingRuleCommand(vacuumer));
        // AddCommand(new UpdateVacuumingRuleCommand(vacuumer));
        // AddCommand(new DeleteVacuumingRuleCommand(vacuumer));
        AddCommand(new ShowVacuumingRuleCommand(vacuumer));
        AddCommand(new ExecuteVacuumingRuleCommand(vacuumer));
    }

    // private sealed class AddVacuumingRuleCommand : AddCommand
    // {
    //     public AddVacuumingRuleCommand(IVacuumer vacuumer, string? description = null) : base(description)
    //     {
    //         var vacuumingRuleArgument = ArgumentBuilder.CreateArgument<string>("vacuuming rule");
    //
    //         AddArgument(vacuumingRuleArgument);
    //         
    //         this.SetHandler((rule) =>
    //         {
    //             vacuumer.AddVacuumingRule(rule)
    //         }, vacuumingRuleArgument);
    //
    //         this.SetHandler(vacuumer.AddVacuumingRule, vacuumingRuleArgument);
    //     }
    // }

    // private sealed class UpdateVacuumingRuleCommand : UpdateCommand
    // {
    //     public UpdateVacuumingRuleCommand(IVacuumer vacuumer, string? description = null) : base(description)
    //     {
    //         var vacuumingRuleIdArgument = ArgumentBuilder.CreateArgument<int>("vacuuming rule id");
    //         var vacuumingRuleArgument = ArgumentBuilder.CreateArgument<string>("vacuuming rule");
    //
    //         AddArgument(vacuumingRuleIdArgument);
    //         AddArgument(vacuumingRuleArgument);
    //
    //         this.SetHandler(vacuumer.UpdateVacuumingRule, vacuumingRuleIdArgument, vacuumingRuleArgument);
    //     }
    // }

    // private sealed class DeleteVacuumingRuleCommand : DeleteCommand
    // {
    //     public DeleteVacuumingRuleCommand(IVacuumer vacuumer, string? description = null) : base(description)
    //     {
    //         var vacuumingRuleIdArgument = ArgumentBuilder.CreateArgument<int>("vacuuming rule id");
    //
    //         AddArgument(vacuumingRuleIdArgument);
    //
    //         this.SetHandler(vacuumer.DeleteVacuumingRule, vacuumingRuleIdArgument);
    //     }
    // }

    // TODO: De to klasser ShowVacuumingRuleCommand og ExecuteVacuumingRuleCommand er næsten kopier af hinanden, fix?
    private sealed class ShowVacuumingRuleCommand : ShowCommand
    {
        public ShowVacuumingRuleCommand(IVacuumer vacuumer, string? description = null) : base(description)
        {
            var idOption = OptionBuilder.CreateIdOption()
                .WithDescription("The id of the vacuuming rule that should be shown");
            var allOption = OptionBuilder.CreateAllOption("Shows all vacuuming rules");

            AddOption(idOption);
            AddOption(allOption);

            AddValidator(commandResult => OptionBuilder.ValidateOneOf(commandResult, idOption, allOption));

            this.SetHandler(context =>
            {
                if (context.ParseResult.HasOption(idOption))
                {
                    var value = context.ParseResult.GetValueForOption(idOption);
                    Console.WriteLine(vacuumer.GetVacuumingRule(value));
                }
                else if (context.ParseResult.HasOption(allOption))
                {
                    Console.WriteLine(vacuumer.GetAllVacuumingRules());
                }
            });

            Description += OptionBuilder.OneOfRequiredText(idOption, allOption);
        }
    }

    private sealed class ExecuteVacuumingRuleCommand : AliasedCommand
    {
        public ExecuteVacuumingRuleCommand(IVacuumer vacuumer, string? description = null)
            : base("execute", "e", description)
        {
            var idOption = OptionBuilder.CreateIdOption()
                .WithDescription("The id of the vacuuming rule that should be executed");
            var allOption = OptionBuilder.CreateAllOption("Executes all vacuuming rules");

            AddOption(idOption);
            AddOption(allOption);

            AddValidator(commandResult => { OptionBuilder.ValidateOneOf(commandResult, idOption, allOption); });

            this.SetHandler(context =>
            {
                if (context.ParseResult.HasOption(idOption))
                {
                    var value = context.ParseResult.GetValueForOption(idOption);
                    vacuumer.RunVacuumingRules(new List<VacuumingRule>(value));
                }
                else if (context.ParseResult.HasOption(allOption))
                {
                    vacuumer.RunAllVacuumingRules();
                }
            });

            Description += OptionBuilder.OneOfRequiredText(idOption, allOption);
        }
    }
}