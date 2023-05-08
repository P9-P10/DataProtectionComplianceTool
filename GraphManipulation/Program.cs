// See https://aka.ms/new-console-template for more information

using System.CommandLine.Builder;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.Data;
using System.Data.SQLite;
using System.Text;
using Dapper;
using GraphManipulation.Commands.Builders;
using GraphManipulation.DataAccess;
using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Decorators;
using GraphManipulation.Decorators.Managers;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Vacuuming;
using Microsoft.EntityFrameworkCore;

namespace GraphManipulation;

// TODO: Når navnet på en entity ændres, mangler der at blive tjekket om det nye navn eksisterer i forvejen, og derfor ikke kan bruges
// TODO: En refactor af managers, så hver manager har en Add(TKey key) i stedet for varierende interfaces ville simplificere kommandoer gevaldigt (de andre værdier kan klares med updates efterfølgende)

public static class Program
{
    private static string configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
    private static bool VerboseOutput = true;

    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            if(args.Length == 1)
                configPath = args[0];
            else
            {
                Console.WriteLine("Received too many arguments. Only a single argument specifying the path of the configuration file expected");
                return;
            }
        }

        try
        {
            Interactive();
        }
        catch (IOException e)
        {
            Console.WriteLine("The given argument is not a valid filepath");
        }
    }

    private static void Interactive()
    {
        ConsoleSetup();

        if (!ConfigSetup(out var configManager))
        {
            return;
        }

        var logger = new PlaintextLogger(configManager);
        var console = new SystemConsole();


        var connectionString = configManager.GetValue("DatabaseConnectionString");
        var context = new GdprMetadataContext(connectionString);
        var dbConnection = new SQLiteConnection(connectionString);

        AddStructureToDatabaseIfNotExists(dbConnection, context);

        var individualMapper = new Mapper<Individual>(context);
        var personalDataColumnMapper = new Mapper<PersonalDataColumn>(context);
        var purposeMapper = new Mapper<Purpose>(context);
        var originMapper = new Mapper<Origin>(context);
        var vacuumingRuleMapper = new Mapper<VacuumingRule>(context);
        var deleteConditionMapper = new Mapper<DeleteCondition>(context);
        var processingMapper = new Mapper<Processing>(context);
        var personalDataMapper = new Mapper<PersonalData>(context);

        var vacuumer = new Vacuumer(personalDataColumnMapper, new SqliteQueryExecutor(dbConnection));
        var loggingVacuumer = new LoggingVacuumer(vacuumer, logger);

        var individualsManager = new IndividualsManager(individualMapper, new Mapper<ConfigKeyValue>(context));
        var personalDataManager = new PersonalDataManager(personalDataColumnMapper, purposeMapper, originMapper,
            personalDataMapper, individualMapper);
        var purposesManager = new PurposeManager(purposeMapper, deleteConditionMapper);
        var originsManager = new OriginsManager(originMapper);
        var vacuumingRulesManager = new VacuumingRuleManager(vacuumingRuleMapper, purposeMapper, loggingVacuumer);
        var deleteConditionsManager = new DeleteConditionsManager(deleteConditionMapper);
        var processingsManager =
            new ProcessingsManager(processingMapper, purposeMapper, personalDataColumnMapper);

        var decoratedIndividualsManager = new IndividualsManagerDecorator(individualsManager, logger);
        var decoratedPersonalDataManager = new PersonalDataManagerDecorator(personalDataManager, logger);
        var decoratedPurposesManager = new PurposeManagerDecorator(purposesManager, logger);
        var decoratedOriginsManager = new OriginsManagerDecorator(originsManager, logger);
        var decoratedVacuumingRulesManager = new VacuumingRuleManagerDecorator(vacuumingRulesManager, logger);
        var decoratedDeleteConditionsManager =
            new DeleteConditionsManagerDecorator(deleteConditionsManager, logger);
        var decoratedProcessingsManager = new ProcessingsManagerDecorator(processingsManager, logger);

        var command = CommandLineInterfaceBuilder
            .Build(
                console, decoratedIndividualsManager, decoratedPersonalDataManager,
                decoratedPurposesManager, decoratedOriginsManager, decoratedVacuumingRulesManager,
                decoratedDeleteConditionsManager, decoratedProcessingsManager, logger, configManager
            );

        var cli = new CommandLineBuilder(command)
            .UseHelp("help", "h", "?")
            .UseTypoCorrections()
            .UseParseErrorReporting()
            .Build();

        Run(cli);
    }

    private static void AddStructureToDatabaseIfNotExists(IDbConnection connection, DbContext context)
    {
        connection.Execute(
            CreateStatementManipulator.UpdateCreationScript(context.Database.GenerateCreateScript()));
    }

    private static void Run(Parser cli)
    {
        while (true)
        {
            try
            {
                Console.Write($"{Environment.NewLine}$: ");
                var command = (Console.ReadLine() ?? "").Trim();

                if (!string.IsNullOrEmpty(command))
                {
                    cli.Invoke(command);
                }
            }
            catch (AggregateException e)
            {
                Console.Error.WriteLine(VerboseOutput ? e.ToString() : e.Message);
                
                foreach (var innerException in e.InnerExceptions)
                {
                    Console.Error.WriteLine(VerboseOutput ? innerException.ToString() : innerException.Message);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(VerboseOutput ? e.ToString() : e.Message);

                if (e.InnerException is not null)
                {
                    Console.Error.WriteLine(VerboseOutput ? e.InnerException.ToString() : e.InnerException.Message);
                }
            }
        }
    }

    private static void ConsoleSetup()
    {
        Console.OutputEncoding = Encoding.UTF8;
    }

    private static bool ConfigSetup(out IConfigManager? configManager)
    {
        var configFilePath = configPath;
        var configValues = new Dictionary<string, string>
        {
            { "GraphStoragePath", "" },
            { "BaseURI", "http://www.test.com/" },
            { "OntologyPath", "" },
            { "LogPath", "" },
            { "DatabaseConnectionString", "" },
            { "IndividualsTable", "" }
        };

        configManager = new ConfigManager(configFilePath, configValues);

        if (!IsValidConfig(configManager, configFilePath))
        {
            return false;
        }

        Console.WriteLine($"Using config found at {configFilePath}");
        return true;
    }

    private static bool IsValidConfig(IConfigManager configManager, string configFilePath)
    {
        if (configManager.GetEmptyKeys().Count == 0) return true;

        Console.WriteLine(
            $"Please fill {string.Join(", ", configManager.GetEmptyKeys())} in config file located at: {configFilePath}");
        return false;
    }
}