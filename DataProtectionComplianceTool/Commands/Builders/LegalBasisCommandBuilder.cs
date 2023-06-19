using System.CommandLine;
using GraphManipulation.Commands.Binders;
using GraphManipulation.Factories.Interfaces;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Builders;

public class LegalBasisCommandBuilder : BaseCommandBuilder<string, LegalBasis>
{
    public LegalBasisCommandBuilder(ICommandHandlerFactory commandHandlerFactory) : base(commandHandlerFactory)
    {
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.LegalBasisName, CommandNamer.LegalBasisNameAlias, out var keyOption);
        
        var descriptionOption = OptionBuilder.CreateEntityDescriptionOption<LegalBasis>();
        var newKeyOption = OptionBuilder.CreateNewNameOption<LegalBasis>();
        
        var createLegalBasisBinder = new LegalBasisBinder(keyOption, descriptionOption);
        var updateLegalBasisBinder = new LegalBasisBinder(newKeyOption, descriptionOption);

        return baseCommand
            .WithSubCommands(
                CreateCommand(keyOption, createLegalBasisBinder, new Option[]
                {
                    descriptionOption
                }),
                UpdateCommand(keyOption, updateLegalBasisBinder, new Option[]
                {
                    newKeyOption, descriptionOption
                }));
    }

    protected override void StatusReport(LegalBasis value)
    {
        // Nothing to report on
    }

    protected override Option<string> BuildKeyOption()
    {
        return OptionBuilder.CreateKeyOption<string, LegalBasis>(OptionNamer.Name, OptionNamer.NameAlias);
    }
}