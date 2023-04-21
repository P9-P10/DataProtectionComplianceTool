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
            .WithSubCommand(AddPersonalData(personalDataManager))
            .WithSubCommand(UpdatePersonalData(personalDataManager))
            .WithSubCommand(DeletePersonalData(personalDataManager))
            .WithSubCommand(ListPersonalData(console, personalDataManager))
            .WithSubCommand(ShowPersonalData(console, personalDataManager));
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
            .CreateDescriptionOption("Description of the personal data")
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
            .WithOption(tableOption)
            .WithOption(columnOption)
            .WithOption(joinConditionOption)
            .WithOption(descriptionOption)
            .WithOption(purposeOption);
        
        command.SetHandler((table, column, joinCondition, description, purposes) =>
        {
            var pair = new TableColumnPair(table, column);
            personalDataManager.AddPersonalData(pair, joinCondition, description);
            purposes.ToList().ForEach(purpose => personalDataManager.AddPurpose(pair, purpose));
        }, tableOption, columnOption, joinConditionOption, descriptionOption, purposeOption);

        return command;
    }

    private static Command UpdatePersonalData(IPersonalDataManager personalDataManager)
    {
        var tableOption = BuildTableOption();
        var columnOption = BuildColumnOption();
        
        var descriptionOption = OptionBuilder
            .CreateDescriptionOption("Description of the personal data")
            .WithIsRequired(true);
        
        var command = CommandBuilder
            .BuildUpdateCommand()
            .WithDescription("Updates the personal data with the given values")
            .WithOption(tableOption)
            .WithOption(columnOption)
            .WithOption(descriptionOption);
        
        command.SetHandler((table, column, description) =>
        {
            var pair = new TableColumnPair(table, column);
            personalDataManager.UpdateDescription(pair, description);
        }, tableOption, columnOption, descriptionOption);

        return command;
    }

    private static Command DeletePersonalData(IPersonalDataManager personalDataManager)
    {
        var tableOption = BuildTableOption();
        var columnOption = BuildColumnOption();

        var command = CommandBuilder
            .BuildDeleteCommand()
            .WithDescription("Deletes the specified personal data from the data managed by the system")
            .WithOption(tableOption)
            .WithOption(columnOption);
        
        command.SetHandler((table, column) =>
        {
            personalDataManager.Delete(new TableColumnPair(table, column));
        }, tableOption, columnOption);

        return command;
    }

    private static Command ListPersonalData(IConsole console, IPersonalDataManager personalDataManager)
    {
        var command = CommandBuilder
            .BuildListCommand()
            .WithDescription("Lists the personal data currently managed by the system")
            .WithHandler(_ =>
            {
                personalDataManager
                    .GetAll()
                    .ToList()
                    .ForEach(s => console.WriteLine(s.ToListing()));
            });

        return command;
    }

    private static Command ShowPersonalData(IConsole console, IPersonalDataManager personalDataManager)
    {
        return CommandBuilder.BuildShowCommand();
    }
    
    private static Option<string> BuildTableOption()
    {
        return OptionBuilder
            .CreateTableOption("The table in which the personal data can be found")
            .WithIsRequired(true);
    }
    
    private static Option<string> BuildColumnOption()
    {
        return OptionBuilder
            .CreateColumnOption("The column in which the personal data can be found")
            .WithIsRequired(true);
    }
}