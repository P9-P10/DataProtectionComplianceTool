using System.CommandLine;
using System.CommandLine.Parsing;
using GraphManipulation.Commands.BaseCommands;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.MetadataManagement;

namespace GraphManipulation.Commands.CompositeCommands;

public sealed class MetadataEntriesCommand : AliasedCommand
{
    public MetadataEntriesCommand(IMetadataManager metadataManager, string? description = null)
        : base("metadata-entries", "me", description)
    {
        AddCommand(new AddMetadataEntryCommand(metadataManager));
        AddCommand(new UpdateMetadataEntryCommand(metadataManager));
        AddCommand(new DeleteMetadataEntryCommand(metadataManager));
        AddCommand(new ShowMetadataEntryCommand(metadataManager));
    }

    private static Option<string?> BuildPurposeOption()
    {
        return OptionBuilder.BuildOption<string?>(
            "--purpose",
            "The purpose under which the personal data is stored",
            "-p"
        );
    }

    private static Option<string?> BuildOriginOption()
    {
        return OptionBuilder.BuildOption<string?>(
            "--origin",
            "The origin of the personal data, the place it is collected from",
            "-o"
        );
    }

    private static Option<bool?> BuildLegallyRequiredOption()
    {
        return OptionBuilder.BuildOption<bool?>(
            "--legally-required",
            "Whether or not the data is legally required to be stored",
            "-l"
        );
    }

    private sealed class AddMetadataEntryCommand : AddCommand
    {
        public AddMetadataEntryCommand(IMetadataManager metadataManager, string? description = null) : base(description)
        {
            var targetTableArgument = ArgumentBuilder.BuildStringArgument("target table");
            var targetColumnArgument = ArgumentBuilder.BuildStringArgument("target column");

            AddArgument(targetTableArgument);
            AddArgument(targetColumnArgument);

            var purposeOption = BuildPurposeOption();
            var originOption = BuildOriginOption();
            var legallyRequiredOption = BuildLegallyRequiredOption();

            AddOption(purposeOption);
            AddOption(originOption);
            AddOption(legallyRequiredOption);

            this.SetHandler(
                (table, column, purpose, origin, legallyRequired) =>
                {
                    metadataManager.MarkAsPersonalData(new GDPRMetadata(table, column)
                        { Purpose = purpose, Origin = origin, LegallyRequired = legallyRequired });
                }, targetTableArgument, targetColumnArgument, purposeOption, originOption, legallyRequiredOption);
        }
    }

    private sealed class UpdateMetadataEntryCommand : UpdateCommand
    {
        public UpdateMetadataEntryCommand(IMetadataManager metadataManager, string? description = null) : base(
            description)
        {
            var metadataIdArgument = ArgumentBuilder.BuildIntArgument("metadata id");

            AddArgument(metadataIdArgument);

            var purposeOption = BuildPurposeOption();
            var originOption = BuildOriginOption();
            var legallyRequiredOption = BuildLegallyRequiredOption();

            AddOption(purposeOption);
            AddOption(originOption);
            AddOption(legallyRequiredOption);


            this.SetHandler(
                (metadataId, purpose, origin, legallyRequired) =>
                {
                    if (!(purpose is null && origin is null && legallyRequired is null))
                    {
                        metadataManager.UpdateMetadataEntry(metadataId, new GDPRMetadata
                            { Purpose = purpose, Origin = origin, LegallyRequired = legallyRequired });
                    }
                }, metadataIdArgument, purposeOption, originOption, legallyRequiredOption);
        }
    }

    private sealed class DeleteMetadataEntryCommand : DeleteCommand
    {
        public DeleteMetadataEntryCommand(IMetadataManager metadataManager, string? description = null) : base(
            description)
        {
            var metadataIdArgument = ArgumentBuilder.BuildIntArgument("metadata id");

            AddArgument(metadataIdArgument);

            this.SetHandler(metadataManager.DeleteMetadataEntry, metadataIdArgument);
        }
    }

    private sealed class ShowMetadataEntryCommand : ShowCommand
    {
        public ShowMetadataEntryCommand(IMetadataManager metadataManager, string? description = null) : base(
            description)
        {
            var idOption = OptionBuilder.CreateIdOption()
                .WithDescription("The id of the metadata entry that should be shown");
            var allOption = OptionBuilder.CreateAllOption("Shows all metadata entries");

            var missingDataOption = OptionBuilder.BuildOption<bool>(
                "--missing-data",
                "Shows the metadata entries that have missing fields",
                "-m"
            );

            AddOption(idOption);
            AddOption(allOption);
            AddOption(missingDataOption);

            AddValidator(commandResult =>
            {
                OptionBuilder.ValidateOneOf(commandResult, idOption, allOption, missingDataOption);
            });

            this.SetHandler(context =>
            {
                if (context.ParseResult.HasOption(idOption))
                {
                    var value = context.ParseResult.GetValueForOption(idOption);
                    Console.WriteLine(metadataManager.GetMetadataEntry(value));
                }
                else if (context.ParseResult.HasOption(missingDataOption))
                {
                    Console.WriteLine(metadataManager.GetMetadataWithNullValues());
                }
                else if (context.ParseResult.HasOption(allOption))
                {
                    Console.WriteLine(metadataManager.GetAllMetadataEntries());
                }
            });

            Description += OptionBuilder.OneOfRequiredText(idOption, allOption, missingDataOption);
        }
    }
}