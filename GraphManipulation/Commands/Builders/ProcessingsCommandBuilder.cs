using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class ProcessingsCommandBuilder
{
    public static Command Build(IConsole console, IProcessingsManager processingsManager,
        IPersonalDataManager personalDataManager, IPurposesManager purposesManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.ProcessingsName)
            .WithAlias(CommandNamer.ProcessingsAlias)
            .WithSubCommands(
                Add(console, processingsManager, personalDataManager, purposesManager),
                Update(console, processingsManager),
                Delete(console, processingsManager),
                List(console, processingsManager),
                Show(console, processingsManager)
            );
    }

    private static Command Add(IConsole console, IProcessingsManager processingsManager,
        IPersonalDataManager personalDataManager, IPurposesManager purposesManager)
    {
        return CommandBuilder
            .BuildAddCommand()
            .WithDescription("Adds a record of processing of personal data under a given purpose to the system")
            .WithOption(out var nameOption, BuildNameOption())
            .WithOption(out var descriptionOption,
                OptionBuilder
                    .CreateDescriptionOption()
                    .WithDescription("The description of the processing")
                    .WithGetDefaultValue(() => ""))
            .WithOption(out var pairOption,
                OptionBuilder
                    .CreateTableColumnPairOption()
                    .WithDescription(
                        "The table and column in which the personal data the is being processing is stored"))
            .WithOption(out var purposeOption,
                OptionBuilder
                    .CreateOption<string>("--purpose")
                    .WithAlias("-p")
                    .WithDescription("The purpose under which the personal data is being processed")
                    .WithIsRequired(true))
            .WithHandler(context => Handlers.AddHandlerKey(context, console,
                processingsManager.AddProcessing,
                processingsManager,
                personalDataManager,
                purposesManager,
                nameOption,
                pairOption,
                purposeOption,
                descriptionOption));
    }

    private static Command Update(IConsole console, IProcessingsManager processingsManager)
    {
        return CommandBuilder
            .BuildUpdateCommand()
            .WithDescription("Updates the given processing with the given values")
            .WithOption(out var nameOption, BuildNameOption())
            .WithOption(out var newNameOption,
                OptionBuilder
                    .CreateNewNameOption()
                    .WithDescription("The new name of the processing"))
            .WithOption(out var descriptionOption,
                OptionBuilder
                    .CreateDescriptionOption()
                    .WithDescription("The new description of the processing"))
            .WithHandler(context =>
            {
                Handlers.UpdateHandler(context, console,
                    processingsManager.UpdateDescription,
                    processingsManager,
                    p => p.GetDescription(),
                    nameOption,
                    descriptionOption);

                Handlers.UpdateHandlerUnique(context, console,
                    processingsManager.UpdateName,
                    processingsManager,
                    p => p.GetName(),
                    nameOption,
                    newNameOption);
            });
    }

    private static Command Delete(IConsole console, IProcessingsManager processingsManager)
    {
        return CommandBuilder
            .BuildDeleteCommand()
            .WithDescription("Deletes the given processing from the system")
            .WithOption(out var nameOption, BuildNameOption())
            .WithHandler(context => Handlers.DeleteHandler(context, console,
                processingsManager.Delete,
                processingsManager,
                nameOption));
    }

    private static Command List(IConsole console, IProcessingsManager processingsManager)
    {
        return CommandBuilder
            .BuildListCommand()
            .WithDescription("Lists the processings currently in the system")
            .WithHandler(() => Handlers.ListHandler(console, processingsManager,CommandHeader.ProcessingsHeader));
    }

    private static Command Show(IConsole console, IProcessingsManager processingsManager)
    {
        return CommandBuilder
            .BuildShowCommand()
            .WithDescription("Shows more information about the given processing")
            .WithOption(out var nameOption, BuildNameOption())
            .WithHandler(context => Handlers.ShowHandler(context, console, processingsManager, nameOption));
    }

    private static Option<string> BuildNameOption()
    {
        return OptionBuilder
            .CreateNameOption()
            .WithDescription("The name of the processing")
            .WithIsRequired(true);
    }
}