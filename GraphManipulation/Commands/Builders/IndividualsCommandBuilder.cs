using System.CommandLine;
using GraphManipulation.Commands.Binders;
using GraphManipulation.Factories;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;

namespace GraphManipulation.Commands.Builders;

public class IndividualsCommandBuilder : BaseCommandBuilder<int, Individual>
{
    private readonly IManager<TableColumnPair, PersonalDataColumn> _personalDataColumnManager;

    public IndividualsCommandBuilder(IHandlerFactory handlerFactory, IManagerFactory managerFactory) : base(handlerFactory)
    {
        _personalDataColumnManager = managerFactory.CreateManager<TableColumnPair, PersonalDataColumn>();
    }

    public override Command Build()
    {
        var baseCommand = base.Build(CommandNamer.IndividualsName, CommandNamer.IndividualsAlias, out var keyOption);

        var descriptionOption = OptionBuilder.CreateEntityDescriptionOption<Individual>();

        return baseCommand
            .WithSubCommands(
                CreateCommand(keyOption, new IndividualBinder(keyOption, descriptionOption), new Option[]
                {
                    
                })
            );
    }

    protected override void StatusReport(Individual individual)
    {
        var personalDataColumns = _personalDataColumnManager.GetAll();
        var personalDataOrigins = individual.PersonalDataOrigins?
            .Where(pdo => pdo.PersonalDataColumn is not null).ToList() ?? new List<PersonalDataOrigin>();

        foreach (var pdc in personalDataColumns)
        {
            var pdo = personalDataOrigins.FirstOrDefault(pdo => pdo.PersonalDataColumn!.Equals(pdc));
            
            if (!personalDataOrigins.Any() || pdo?.Origin is null)
            {
                // There exists a personal data column, but the individual does not have a personal data origin for it
                FeedbackEmitter.EmitMissing(individual.Key, $"origin for '{pdc.ToListingIdentifier()}'");
            }
        }
    }

    protected override Option<int> BuildKeyOption()
    {
        return OptionBuilder.CreateKeyOption<int, Individual>(OptionNamer.Id, OptionNamer.IdAlias, "id");
    }
}