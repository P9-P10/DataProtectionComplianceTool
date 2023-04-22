using System.CommandLine;
using GraphManipulation.Commands.BaseBuilders;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class PersonalDataCommandBuilder
{
    public static Command Build(IConsole console, IPersonalDataManager personalDataManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.PersonalDataName)
            .WithAlias(CommandNamer.PersonalDataAlias)
            .WithSubCommands(
                AddPersonalData(personalDataManager),
                UpdatePersonalData(console, personalDataManager),
                DeletePersonalData(console, personalDataManager),
                ListPersonalData(console, personalDataManager),
                ShowPersonalData(console, personalDataManager)
            );
    }

    private static Command AddPersonalData(IPersonalDataManager personalDataManager)
    {
        var tableOption = BuildTableOption();
        var columnOption = BuildColumnOption();

        var joinConditionOption = OptionBuilder
            .CreateOption<string>("--join-condition")
            .WithAlias("-jc")
            .WithDescription("The condition under which the given table can be joined with the individuals table")
            .WithIsRequired(true);

        var descriptionOption = OptionBuilder
            .CreateDescriptionOption<string>()
            .WithDescription("Description of the personal data")
            .WithGetDefaultValue(() => "");

        var purposeOption = OptionBuilder
            .CreateOption<IEnumerable<string>>("--purpose")
            .WithAlias("-p")
            .WithDescription("The purpose under which the personal data is stored")
            .WithIsRequired(true)
            .WithAllowMultipleArguments(true)
            .WithArity(ArgumentArity.OneOrMore);

        var command = CommandBuilder
            .BuildAddCommand()
            .WithDescription("Adds the personal data found in the given column to the data managed by the system")
            .WithOptions(
                tableOption,
                columnOption,
                joinConditionOption,
                descriptionOption,
                purposeOption
            );

        command.SetHandler((table, column, joinCondition, description, purposes) =>
        {
            var pair = new TableColumnPair(table, column);
            personalDataManager.AddPersonalData(pair, joinCondition, description);
            purposes.ToList().ForEach(purpose => personalDataManager.AddPurpose(pair, purpose));
        }, tableOption, columnOption, joinConditionOption, descriptionOption, purposeOption);

        return command;
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
                console.WriteLine($"Could not find any personal data using \"{table}\" and \"{column}\"");
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
                    console.WriteLine($"Could not find any personal data using \"{table}\" and \"{column}\"");
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
                console.WriteLine($"Could not find any personal data using \"{table}\" and \"{column}\"");
                return;
            }

            console.WriteLine(value.ToListing());
        }, tableOption, columnOption);

        return command;
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