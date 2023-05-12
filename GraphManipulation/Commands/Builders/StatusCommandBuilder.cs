using System.CommandLine;
using GraphManipulation.Commands.Helpers;

namespace GraphManipulation.Commands.Builders;

// TODO: Implementer status kommando

public class StatusCommandBuilder
{
    public StatusCommandBuilder()
    {
        
    }
    
    public Command Build()
    {
        var command = CommandBuilder
            .BuildStatusCommand()
            .WithDescription("Shows the status for the system");

        return command;
    }
}