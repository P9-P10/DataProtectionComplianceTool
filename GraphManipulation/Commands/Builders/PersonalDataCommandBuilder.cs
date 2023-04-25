using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class PersonalDataCommandBuilder
{
    public static Command Build(IConsole console, IPersonalDataManager personalDataManager,
        IPurposesManager purposesManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.PersonalDataName)
            .WithAlias(CommandNamer.PersonalDataAlias)
            .WithSubCommands(
                AddPersonalData(console, personalDataManager, purposesManager),
                UpdatePersonalData(console, personalDataManager),
                DeletePersonalData(console, personalDataManager),
                ListPersonalData(console, personalDataManager),
                ShowPersonalData(console, personalDataManager),
                AddPurpose(console, personalDataManager, purposesManager),
                RemovePurpose(console, personalDataManager)
            );
    }

    private static Command AddPersonalData(IConsole console, IPersonalDataManager personalDataManager,
        IPurposesManager purposesManager)
    {
        return CommandBuilder
            .BuildAddCommand()
            .WithDescription(
                "Adds the personal data found in the given table and column to the data managed by the system")
            .WithOption(out var pairOption, BuildPairOption())
            .WithOption(out var joinConditionOption,
                OptionBuilder
                    .CreateOption<string>("--join-condition")
                    .WithAlias("-jc")
                    .WithDescription(
                        "The condition under which the given table can be joined with the individuals table")
                    .WithIsRequired(true))
            .WithOption(out var descriptionOption,
                OptionBuilder
                    .CreateDescriptionOption<string>()
                    .WithDescription("Description of the personal data")
                    .WithGetDefaultValue(() => ""))
            .WithOption(out var purposeOption, BuildPurposeListOption())
            .WithHandler(context =>
            {
                Handlers.AddHandler(context, console,
                    personalDataManager.AddPersonalData,
                    pairOption,
                    joinConditionOption,
                    descriptionOption);

                Handlers.UpdateHandlerWithKeyList(context, console,
                    personalDataManager.AddPurpose,
                    personalDataManager,
                    purposesManager,
                    column => column.GetPurposes().Select(p => p.GetName()),
                    pairOption,
                    purposeOption
                );
            });
    }

    private static Command UpdatePersonalData(IConsole console, IPersonalDataManager personalDataManager)
    {
        return CommandBuilder
            .BuildUpdateCommand()
            .WithDescription("Updates the personal data with the given values")
            .WithOption(out var pairOption, BuildPairOption())
            .WithOption(out var descriptionOption,
                OptionBuilder
                    .CreateDescriptionOption<string>()
                    .WithDescription("Description of the personal data"))
            .WithHandler(context => 
                Handlers.UpdateHandler(context, console,
                    personalDataManager.UpdateDescription,
                    personalDataManager,
                    c => c.GetDescription(),
                    pairOption,
                    descriptionOption
                )
            );
    }

    private static Command DeletePersonalData(IConsole console, IPersonalDataManager personalDataManager)
    {
        return CommandBuilder
            .BuildDeleteCommand()
            .WithDescription("Deletes the specified personal data from the data managed by the system")
            .WithOption(out var pairOption, BuildPairOption())
            .WithHandler(context =>
                Handlers.DeleteHandler(context, console, personalDataManager.Delete, personalDataManager, pairOption));
    }

    private static Command ListPersonalData(IConsole console, IPersonalDataManager personalDataManager)
    {
        return CommandBuilder
            .BuildListCommand()
            .WithDescription("Lists the personal data currently managed by the system")
            .WithHandler(() => Handlers.ListHandler(console, personalDataManager));
    }

    private static Command ShowPersonalData(IConsole console, IPersonalDataManager personalDataManager)
    {
        return CommandBuilder
            .BuildShowCommand()
            .WithDescription("Shows information about the personal data found in the given table and column")
            .WithOption(out var pairOption, BuildPairOption())
            .WithHandler(context => Handlers.ShowHandler(context, console, personalDataManager, pairOption));
    }

    private static Command AddPurpose(IConsole console, IPersonalDataManager personalDataManager,
        IPurposesManager purposesManager)
    {
        return CommandBuilder
            .BuildAddCommand("purpose")
            .WithDescription("Adds the given purpose(s) to the personal data entry")
            .WithOption(out var pairOption, BuildPairOption())
            .WithOption(out var purposeOption, BuildPurposeListOption())
            .WithHandler(context => Handlers.UpdateHandlerWithKeyList(context, console,
                personalDataManager.AddPurpose,
                personalDataManager,
                purposesManager,
                column => column.GetPurposes().Select(p => p.GetName()),
                pairOption,
                purposeOption
            ));
    }

    private static Command RemovePurpose(IConsole console, IPersonalDataManager personalDataManager)
    {
        return CommandBuilder
            .BuildRemoveCommand("purpose")
            .WithDescription("Removes the given purpose(s) from the personal data entry")
            .WithOption(out var pairOption, BuildPairOption())
            .WithOption(out var purposeOption, BuildPurposeListOption())
            .WithHandler(context => Handlers.RemoveHandlerKeyList(context, console,
                personalDataManager.RemovePurpose,
                personalDataManager,
                column => column.GetPurposes().Select(p => p.GetName()),
                pairOption,
                purposeOption));
    }

    private static Option<TableColumnPair> BuildPairOption()
    {
        return OptionBuilder
            .CreateTableColumnPairOption()
            .WithDescription("The table and column in which the personal data can be found");
    }

    private static Option<IEnumerable<string>> BuildPurposeListOption()
    {
        return OptionBuilder
            .CreateOption<IEnumerable<string>>("--purpose")
            .WithAlias("-p")
            .WithDescription("The purpose(s) under which the personal data is stored")
            .WithIsRequired(true)
            .WithAllowMultipleArguments(true)
            .WithArity(ArgumentArity.OneOrMore);
    }
}