// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using System.CommandLine.IO;
using System.Text;
using GraphManipulation.Commands.Builders;
using GraphManipulation.DataAccess;
using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Decorators.Managers;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using Sharprompt;
using Symbol = Sharprompt.Symbol;

namespace GraphManipulation;

// TODO: Manglende decorator til IndividualsManager

// TODO: Skal vi have en kommando til at sætte vores database op? Hvis ja, så skal den laves
// TODO: Skal vi have en kommando til at eksekvere vacuuming? Hvis ja, så skal den laves

// TODO: IProcessingManager skal ikke kunne opdatere purpose. Enten skal den funktionalitet helt væk (fjernes fra interfacen) eller også skal man kunne tilføje og fjerne flere purposes (på samme måde som i IPersonalDataManager)
// TODO: Lige nu har VacuumingRule en liste af purposes, men igennem IVacuumingRulesManager kan den kun få ét purpose. Enten skal den kun have ét purpose, eller også skal det være muligt at tilføje og fjerne flere purposes (på samme måde som i IPersonalDataManager)

// TODO: Når navnet på en entity ændres, mangler der at blive tjekket om det nye navn eksisterer i forvejen, og derfor ikke kan bruges
// TODO: En refactor af managers, så hver manager har en Add(TKey key) i stedet for varierende interfaces ville simplificere kommandoer gevaldigt (de andre værdier kan klares med updates efterfølgende)
// TODO: Når vi udskriver lister af entities, ville det være brugbart hvis listen havde en header (ala. "Name, Description, ...") så brugeren har nemmere ved at forstå hvad de kigger på

public static class Program
{
    public static void Main()
    {
        Interactive();
    }

    private static void Interactive()
    {
        ConsoleSetup();

        var configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
        Dictionary<string,string> configValues= new Dictionary<string, string>
        {
            {"GraphStoragePath", ""},
            {"BaseURI", "http://www.test.com/"},
            {"OntologyPath", ""},
            {"LogPath", ""},
            {"DatabaseConnectionString", ""},
            {"IndividualsTable", ""}
        };
        var configManager = new ConfigManager(configFilePath,configValues);

        if (!ConfigSetup(configManager, configFilePath))
        {
            return;
        }

        Console.WriteLine($"Using config found at {configFilePath}");

        var logger = new PlaintextLogger(configManager);
        var console = new SystemConsole();

        var context = new GdprMetadataContext(configManager.GetValue("DatabaseConnectionString"));

        var individualMapper = new Mapper<Individual>(context);
        var personalDataColumnMapper = new Mapper<PersonalDataColumn>(context);
        var purposeMapper = new Mapper<Purpose>(context);
        var originMapper = new Mapper<Origin>(context);
        var vacuumingRuleMapper = new Mapper<VacuumingRule>(context);
        var deleteConditionMapper = new Mapper<DeleteCondition>(context);
        var processingMapper = new Mapper<Processing>(context);
        var personalDataMapper = new Mapper<PersonalData>(context);

        var individualsManager = new IndividualsManager(individualMapper);
        var personalDataManager = new PersonalDataManager(personalDataColumnMapper, purposeMapper, originMapper,
            personalDataMapper, individualMapper);
        var purposesManager = new PurposeManager(purposeMapper, deleteConditionMapper);
        var originsManager = new OriginsManager(originMapper);
        var vacuumingRulesManager = new VacuumingRuleManager(vacuumingRuleMapper, purposeMapper);
        var deleteConditionsManager = new DeleteConditionsManager(deleteConditionMapper);
        var processingsManager = new ProcessingsManager(processingMapper, purposeMapper, personalDataColumnMapper);

        // var decoratedIndividualsManager = new IndividualsManagerDecorator(individualsManager, logger);
        var decoratedPersonalDataManager = new PersonalDataManagerDecorator(personalDataManager, logger);
        var decoratedPurposesManager = new PurposeManagerDecorator(purposesManager, logger);
        var decoratedOriginsManager = new OriginsManagerDecorator(originsManager, logger);
        var decoratedVacuumingRulesManager = new VacuumingRuleManagerDecorator(vacuumingRulesManager, logger);
        var decoratedDeleteConditionsManager = new DeleteConditionsManagerDecorator(deleteConditionsManager, logger);
        var decoratedProcessingsManager = new ProcessingsManagerDecorator(processingsManager, logger);

        var cli = CommandLineInterfaceBuilder
            .Build(
                console, individualsManager, decoratedPersonalDataManager,
                decoratedPurposesManager, decoratedOriginsManager, decoratedVacuumingRulesManager,
                decoratedDeleteConditionsManager, decoratedProcessingsManager, logger, configManager
            );

        Run(cli);
    }

    private static void Run(Command cli)
    {
        var flag = true;

        do
        {
            try
            {
                var command = Prompt.Input<string>("");
                cli.Invoke(command);
            }
            catch (PromptCanceledException)
            {
                flag = false;
                Console.WriteLine("Goodbye!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        } while (flag);
    }

    private static void ConsoleSetup()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Prompt.ThrowExceptionOnCancel = true;
        Prompt.Symbols.Prompt = new Symbol("$", "$");
    }

    private static bool ConfigSetup(IConfigManager configManager, string configFilePath)
    {
        try
        {
            if (configManager.GetEmptyKeys().Count > 0)
            {
                Console.WriteLine(
                    $"Please fill {string.Join(",", configManager.GetEmptyKeys())} in config file located at: {configFilePath}");
                return false;
            }
        }
        catch (KeyNotFoundException exception)
        {
            Console.WriteLine(exception.Message);
            return false;
        }

        return true;
    }
}