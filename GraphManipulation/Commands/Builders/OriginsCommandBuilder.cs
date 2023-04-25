using System.CommandLine;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;

namespace GraphManipulation.Commands.Builders;

public static class OriginsCommandBuilder
{
    public static Command Build(IConsole console, IOriginsManager originsManager)
    {
        return CommandBuilder.CreateCommand(CommandNamer.OriginsName)
            .WithAlias(CommandNamer.OriginsAlias)
            .WithSubCommands(
                Add(console, originsManager),
                Update(console, originsManager),
                Delete(console, originsManager),
                List(console, originsManager),
                Show(console, originsManager)
            );
    }

    private static Command Add(IConsole console, IOriginsManager originsManager)
    {
        return CommandBuilder
            .BuildAddCommand()
            .WithDescription("Adds the given origin to the system")
            .WithOption(out var nameOption, BuildNameOption())
            .WithOption(out var descriptionOption,
                OptionBuilder
                    .CreateDescriptionOption<string>()
                    .WithDescription("The description of the origin")
                    .WithGetDefaultValue(() => ""))
            .WithHandler(context => Handlers.AddHandler(context, console,
                originsManager.Add, originsManager, nameOption, descriptionOption));
    }

    private static Command Update(IConsole console, IOriginsManager originsManager)
    {
        return CommandBuilder
            .BuildUpdateCommand()
            .WithDescription("Updates the given origin with the given values")
            .WithOption(out var nameOption, BuildNameOption())
            .WithOption(out var newNameOption, 
                OptionBuilder
                    .CreateNewNameOption<string>()
                    .WithDescription("The new name of the origin"))
            .WithOption(out var descriptionOption,
                OptionBuilder.CreateDescriptionOption<string>()
                    .WithDescription("The new description of the origin"))
            .WithHandler(context =>
            {
                Handlers.UpdateHandler(context, console, 
                    originsManager.UpdateDescription, 
                    originsManager, 
                    origin => origin.GetDescription(),
                    nameOption, descriptionOption);
                
                Handlers.UpdateHandler(context, console, 
                    originsManager.UpdateName, 
                    originsManager, 
                    origin => origin.GetName(),
                    nameOption, newNameOption);
            });
    }

    private static Command Delete(IConsole console, IOriginsManager originsManager)
    {
        return CommandBuilder.BuildDeleteCommand();
    }

    private static Command List(IConsole console, IOriginsManager originsManager)
    {
        return CommandBuilder.BuildListCommand();
    }

    private static Command Show(IConsole console, IOriginsManager originsManager)
    {
        return CommandBuilder.BuildShowCommand();
    }

    private static Option<string> BuildNameOption()
    {
        return OptionBuilder
            .CreateNameOption()
            .WithDescription("The name of the origin")
            .WithIsRequired(true);
    }
}