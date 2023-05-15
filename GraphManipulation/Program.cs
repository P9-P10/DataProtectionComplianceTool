// See https://aka.ms/new-console-template for more information

using System.Data;
using System.Data.SQLite;
using System.Text;
using Dapper;
using GraphManipulation.Commands;
using GraphManipulation.DataAccess;
using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Decorators;
using GraphManipulation.Helpers;
using GraphManipulation.Logging;
using GraphManipulation.Models;
using GraphManipulation.Vacuuming;
using Microsoft.EntityFrameworkCore;

namespace GraphManipulation;

public class VariousFactory : IVacuumerFactory, ILoggerFactory, IConfigManagerFactory
{
    private readonly IVacuumer _vacuumer;
    private readonly ILogger _logger;
    private readonly IConfigManager _configManager;

    public VariousFactory(IVacuumer vacuumer, ILogger logger, IConfigManager configManager)
    {
        _vacuumer = vacuumer;
        _logger = logger;
        _configManager = configManager;
    }
    public IVacuumer CreateVacuumer()
    {
        return _vacuumer;
    }

    public ILogger CreateLogger()
    {
        return _logger;
    }

    public IConfigManager CreateConfigManager()
    {
        return _configManager;
    }
}
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
        
        var connectionString = configManager.GetValue("DatabaseConnectionString");
        var context = new GdprMetadataContext(connectionString);
        var dbConnection = new SQLiteConnection(connectionString);

        AddStructureToDatabaseIfNotExists(dbConnection, context);
        
        var personalDataColumnMapper = new Mapper<PersonalDataColumn>(context);
        var purposeMapper = new Mapper<Purpose>(context);
        var originMapper = new Mapper<Origin>(context);
        var vacuumingRuleMapper = new Mapper<VacuumingRule>(context);
        var deleteConditionMapper = new Mapper<StorageRule>(context);
        var processingMapper = new Mapper<Processing>(context);
        var personalDataOriginMapper = new Mapper<PersonalDataOrigin>(context);

        var vacuumer = new Vacuumer(purposeMapper, new SqliteQueryExecutor(dbConnection));
        var loggingVacuumer = new LoggingVacuumer(vacuumer, logger);

        VariousFactory variousFactory = new VariousFactory(loggingVacuumer, logger, configManager);
        IManagerFactory managerFactory = new LoggingManagerFactory(new ManagerFactory(context), logger);
        ComponentFactory componentFactory =
            new ComponentFactory(managerFactory, variousFactory, variousFactory, variousFactory);
        CommandLineInterface commandLineInterface = new CommandLineInterface(componentFactory);
        Run(commandLineInterface);

    }

    private static void AddStructureToDatabaseIfNotExists(IDbConnection connection, DbContext context)
    {
        connection.Execute(
            CreateStatementManipulator.UpdateCreationScript(context.Database.GenerateCreateScript()));
    }

    private static void Run(CommandLineInterface cli)
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