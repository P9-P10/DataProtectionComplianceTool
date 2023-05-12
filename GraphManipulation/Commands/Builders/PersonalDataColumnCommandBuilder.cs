using System.CommandLine;
using GraphManipulation.Commands.Builders.Binders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders;

public class PersonalDataColumnCommandBuilder : BaseCommandBuilder<TableColumnPair, PersonalDataColumn>
{
    private readonly IManager<string, Purpose> _purposesManager;
    private readonly IManager<string, Origin> _originsManager;
    private readonly IManager<int, Individual> _individualsManager;

    public PersonalDataColumnCommandBuilder(
        IConsole console,
        IManager<TableColumnPair, PersonalDataColumn> personalDataColumnManager,
        IManager<string, Purpose> purposesManager,
        IManager<string, Origin> originsManager,
        IManager<int, Individual> individualsManager) : base(console, personalDataColumnManager)
    {
        _purposesManager = purposesManager;
        _originsManager = originsManager;
        _individualsManager = individualsManager;
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.PersonalDataName, CommandNamer.PersonalDataAlias, out var keyOption);

        var descriptionOption = BuildDescriptionOption();

        var defaultValueOption = OptionBuilder
            .CreateOption<string>(OptionNamer.DefaultValue)
            .WithAlias(OptionNamer.DefaultValueAlias)
            .WithDescription("The default value that attributes in the column should receive upon deletion");

        var purposeListOption = OptionBuilder
            .CreatePurposeListOption()
            .WithDescription("The purpose(s) under which the personal data is stored");

        var createBinder = new PersonalDataColumnBinder(
            keyOption,
            descriptionOption,
            purposeListOption,
            defaultValueOption,
            _purposesManager);

        var updateBinder = new PersonalDataColumnBinder(
            keyOption,
            descriptionOption,
            purposeListOption,
            defaultValueOption,
            _purposesManager);

        var purposeListChangesCommands = BuildListChangesCommand(
            keyOption, purposeListOption, _purposesManager,
            column => column.Purposes ?? new List<Purpose>(),
            (column, purposes) => column.Purposes = purposes);

        return baseCommand
            .WithSubCommands(
                CreateCommand(keyOption, createBinder, new Option[]
                {
                    descriptionOption, defaultValueOption, purposeListOption
                }),
                UpdateCommand(keyOption, updateBinder, new Option[]
                {
                    descriptionOption, defaultValueOption, purposeListOption
                }),
                purposeListChangesCommands.Add,
                purposeListChangesCommands.Remove
            );
    }

    protected override Option<TableColumnPair> BuildKeyOption()
    {
        return OptionBuilder.CreateTableColumnPairOption();
    }
    
    // TODO: SetOriginOf og ShowOriginOf skal lige implementeres igen

    // private static Command SetOriginOf(IConsole console, IPersonalDataManager personalDataManager,
    //     IOriginsManager originsManager, IIndividualsManager individualsManager)
    // {
    //     return CommandBuilder
    //         .BuildSetCommand("origin")
    //         .WithDescription("Sets the origin for the given personal data for the given individual to the given origin")
    //         .WithOption(out var pairOption, BuildPairOption())
    //         .WithOption(out var individualOption,
    //             OptionBuilder
    //                 .CreateIdOption()
    //                 .WithDescription("The id of the individual")
    //                 .WithIsRequired(true))
    //         .WithOption(out var originOption,
    //             OptionBuilder
    //                 .CreateOption<string>(OptionNamer.Origin)
    //                 .WithAlias(OptionNamer.OriginAlias)
    //                 .WithDescription("The origin from which the personal data was retrieved")
    //                 .WithIsRequired(true))
    //         .WithHandler(context => Handlers.SetHandlerKey(context, console,
    //             personalDataManager.SetOriginOf,
    //             personalDataManager,
    //             individualsManager,
    //             originsManager,
    //             (pair, id) =>
    //             {
    //                 var origin = personalDataManager.GetOriginOf(pair, id);
    //                 return origin is null ? "" : origin.GetName();
    //             },
    //             pairOption, individualOption, originOption));
    // }
    //
    // private static Command ShowOriginOf(IConsole console, IPersonalDataManager personalDataManager,
    //     IIndividualsManager individualsManager)
    // {
    //     return CommandBuilder
    //         .BuildShowCommand("origin")
    //         .WithDescription("Shows the origin of the given individual's personal data")
    //         .WithOption(out var pairOption, BuildPairOption())
    //         .WithOption(out var individualOption,
    //             OptionBuilder
    //                 .CreateIdOption()
    //                 .WithDescription("The id of the individual")
    //                 .WithIsRequired(true))
    //         .WithHandler(context => Handlers.ShowHandler(context, console,
    //             personalDataManager,
    //             individualsManager,
    //             (pair, id) => personalDataManager.GetOriginOf(pair, id)
    //                           ?? throw new CommandException($"Could not find origin of {pair} and {id}"),
    //             pairOption,
    //             individualOption));
    // }
}