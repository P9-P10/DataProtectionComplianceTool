// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using System.CommandLine.IO;
using System.Text;
using GraphManipulation.Commands.Builders;
using GraphManipulation.DataAccess;
using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using Sharprompt;
using Symbol = Sharprompt.Symbol;

namespace GraphManipulation;

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
        var configManager = new ConfigManager(configFilePath);

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

        var cli = CommandLineInterfaceBuilder
            .Build(
                console, individualsManager, personalDataManager,
                purposesManager, originsManager, vacuumingRulesManager,
                deleteConditionsManager, processingsManager, logger, configManager
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
            if (ConfigFileHasEmptyValues(configManager))
            {
                Console.WriteLine($"Please fill empty values in config file located at: {configFilePath}");
                return false;
            }
        }
        catch (KeyNotFoundException)
        {
            Console.WriteLine(
                $"Please make sure that the config file located at {configFilePath} is correctly set up, " +
                $"keys seem to be missing");
            return false;
        }

        return true;
    }

    private static bool ConfigFileHasEmptyValues(IConfigManager configManager)
    {
        return string.IsNullOrEmpty(configManager.GetValue("DatabaseConnectionString")) ||
               string.IsNullOrEmpty(configManager.GetValue("IndividualsTable")) ||
               string.IsNullOrEmpty(configManager.GetValue("LogPath"));
    }
}