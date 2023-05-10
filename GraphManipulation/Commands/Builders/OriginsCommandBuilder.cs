using System.CommandLine;
using GraphManipulation.Commands.Builders.Binders;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers.Interfaces;
using GraphManipulation.Models;

namespace GraphManipulation.Commands.Builders;

public class OriginsCommandBuilder : BaseCommandBuilder<IOriginsManager, string, Origin>
{
    public OriginsCommandBuilder(IConsole console, IOriginsManager manager) : base(console, manager)
    {
    }

    public Command Build()
    {
        const string subject = "origin";
        const string subjects = "origins";
        
        return CommandBuilder.CreateCommand(CommandNamer.OriginsName)
            .WithAlias(CommandNamer.OriginsAlias)
            .WithSubCommands(Add(), Update(), 
                DeleteCommand(subject, BuildKeyOption()),
                ListCommand(subjects),
                ShowCommand(subject, BuildKeyOption())
            );
    }
    
    private Command Add()
    {
        var command = CommandBuilder
            .BuildAddCommand()
            .WithDescription("Adds the given origin to the system")
            .WithOption(out var nameOption, BuildKeyOption())
            .WithOption(out var descriptionOption,
                OptionBuilder
                    .CreateDescriptionOption()
                    .WithDescription("The description of the origin")
                    .WithGetDefaultValue(() => ""));

        command.SetHandler(CreateHandler, nameOption, new OriginBinder(nameOption, descriptionOption));

        return command;
    }

    private Command Update()
    {
        var command = CommandBuilder
            .BuildUpdateCommand()
            .WithDescription("Updates the given origin with the given values")
            .WithOption(out var nameOption, BuildKeyOption())
            .WithOption(out var newNameOption,
                OptionBuilder
                    .CreateNewNameOption()
                    .WithDescription("The new name of the origin"))
            .WithOption(out var descriptionOption,
                OptionBuilder.CreateDescriptionOption()
                    .WithDescription("The new description of the origin"));

        command.SetHandler(UpdateHandler, nameOption, new OriginBinder(newNameOption, descriptionOption));

        return command;
    }

    private Option<string> BuildKeyOption()
    {
        return base.BuildKeyOption(OptionNamer.Name, OptionNamer.NameAlias, "The name of the origin");
    }
}