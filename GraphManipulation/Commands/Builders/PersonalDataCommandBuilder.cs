using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class PersonalDataCommandBuilder
{
    public static Command Build(IConsole console, IPersonalDataManager personalDataManager, IPurposesManager purposesManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.PersonalDataName)
            .WithAlias(CommandNamer.PersonalDataAlias)
            .WithSubCommands(
                AddPersonalData(console, personalDataManager, purposesManager),
                UpdatePersonalData(console, personalDataManager),
                DeletePersonalData(console, personalDataManager),
                ListPersonalData(console, personalDataManager),
                ShowPersonalData(console, personalDataManager)
            );
    }

    private static Command AddPersonalData(IConsole console, IPersonalDataManager personalDataManager, IPurposesManager purposesManager)
    {
        return CommandBuilder
            .BuildAddCommand()
            .WithDescription(
                "Adds the personal data found in the given table and column to the data managed by the system")
            .WithOption(out var pairOption,
                OptionBuilder
                    .CreateTableColumnPairOption()
                    .WithDescription("The table and column in which the personal data can be found"))
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
            .WithOption(out var purposeOption,
                OptionBuilder
                    .CreateOption<IEnumerable<string>>("--purpose")
                    .WithAlias("-p")
                    .WithDescription("The purpose(s) under which the personal data is stored")
                    .WithIsRequired(true)
                    .WithAllowMultipleArguments(true)
                    .WithArity(ArgumentArity.OneOrMore))
            .WithHandler(new CommandHandler()
                .WithHandle(context => BaseBuilder.AddHandler(context, console,
                    personalDataManager.AddPersonalData,
                    pairOption,
                    joinConditionOption,
                    descriptionOption))
                .WithHandle(context => BaseBuilder.UpdateHandlerWithKeyList(context, console,
                    personalDataManager.AddPurpose, 
                    personalDataManager, 
                    purposesManager, 
                    column => column.GetPurposes().Select(p => p.GetName()),
                    pairOption, 
                    purposeOption
                ))
            );
    }

    private static Command UpdatePersonalData(IConsole console, IPersonalDataManager personalDataManager)
    {
        var tableOption = BuildTableOption();
        var columnOption = BuildColumnOption();

        var descriptionOption = OptionBuilder
            .CreateDescriptionOption<string?>()
            .WithDescription("Description of the personal data");

        var command = CommandBuilder
            .BuildUpdateCommand()
            .WithDescription("Updates the personal data with the given values")
            .WithOptions(tableOption, columnOption, descriptionOption);

        command.SetHandler((table, column, description) =>
        {
            var pair = new TableColumnPair(table, column);
            var old = personalDataManager.Get(pair);

            if (old is null)
            {
                console.WriteLine(BuildFailureToFindMessage(table, column));
                return;
            }

            if (description is not null && old.GetDescription() != description)
            {
                personalDataManager.UpdateDescription(pair, description);
            }
        }, tableOption, columnOption, descriptionOption);

        return command;
    }

    private static Command DeletePersonalData(IConsole console, IPersonalDataManager personalDataManager)
    {
        var tableOption = BuildTableOption();
        var columnOption = BuildColumnOption();

        var command = CommandBuilder
            .BuildDeleteCommand()
            .WithDescription("Deletes the specified personal data from the data managed by the system")
            .WithOptions(tableOption, columnOption);

        command.SetHandler((table, column) =>
            {
                var pair = new TableColumnPair(table, column);
                var value = personalDataManager.Get(pair);

                if (value is null)
                {
                    console.WriteLine(BuildFailureToFindMessage(table, column));
                    return;
                }
                
                personalDataManager.Delete(pair);
            },
            tableOption, columnOption);

        return command;
    }

    private static Command ListPersonalData(IConsole console, IPersonalDataManager personalDataManager)
    {
        return CommandBuilder
            .BuildListCommand(console, personalDataManager)
            .WithDescription("Lists the personal data currently managed by the system");
    }

    private static Command ShowPersonalData(IConsole console, IPersonalDataManager personalDataManager)
    {
        var tableOption = BuildTableOption();
        var columnOption = BuildColumnOption();

        var command = CommandBuilder
            .BuildShowCommand()
            .WithDescription("Shows information about the personal data found in the given table and column")
            .WithOptions(tableOption, columnOption);
        
        command.SetHandler((table, column) =>
        {
            var pair = new TableColumnPair(table, column);
            var value = personalDataManager.Get(pair);

            if (value is null)
            {
                console.WriteLine(BuildFailureToFindMessage(table, column));
                return;
            }

            console.WriteLine(value.ToListing());
        }, tableOption, columnOption);

        return command;
    }

    private static string BuildFailureToFindMessage(string table, string column)
    {
        return CommandBuilder.BuildFailureToFindMessage("any personal data", $"{table}\" and \"{column}");
    }

    private static Option<string> BuildTableOption()
    {
        return OptionBuilder
            .CreateTableOption()
            .WithDescription("The table in which the personal data can be found")
            .WithIsRequired(true);
    }

    private static Option<string> BuildColumnOption()
    {
        return OptionBuilder
            .CreateColumnOption()
            .WithDescription("The column in which the personal data can be found")
            .WithIsRequired(true);
    }
}