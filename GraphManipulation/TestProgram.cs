using System.Data;
using System.Data.SQLite;
using System.Text;
using Dapper;
using GraphManipulation.Commands;
using GraphManipulation.DataAccess;
using GraphManipulation.Factories;
using GraphManipulation.Factories.Interfaces;
using GraphManipulation.Logging;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Utility;
using GraphManipulation.Vacuuming;
using Microsoft.EntityFrameworkCore;

namespace GraphManipulation;

public class TestProgram
{
    public string configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
    private readonly bool VerboseOutput = true;
    public CommandLineInterface Cli { get; set; }

    public void Start(string config = "")
    {
        if (config != "")
        {
            configPath = config;
        }

        ConsoleSetup();

        try
        {
            if (!ConfigSetup(out var configManager))
            {
                throw new Exception("Configuration setup failed");
            }


            var logger = new PlaintextLogger(configManager);

            var connectionString = configManager.GetValue("DatabaseConnectionString");
            var context = new GdprMetadataContext(connectionString);
            var dbConnection = new SQLiteConnection(connectionString);

            AddStructureToDatabaseIfNotExists(dbConnection, context);
            var purposeMapper = new Mapper<Purpose>(context);

            var vacuumer = new Vacuumer(purposeMapper, new SqliteQueryExecutor(dbConnection));

            IManagerFactory managerFactory = new LoggingManagerFactory(new ManagerFactory(context), logger);
            IConfigManagerFactory configManagerFactory = new ConfigManagerFactory(configManager);
            ILoggerFactory loggerFactory = new PlaintextLoggerFactory(configManagerFactory);
            IVacuumerFactory vacuumerFactory = new LoggingVacuumerFactory(new VacuumerFactory(vacuumer), logger);

            Cli = new CommandLineInterface(managerFactory, loggerFactory, vacuumerFactory);
            Console.Write($"{Environment.NewLine}$: ");
        }
        catch (IOException e)
        {
            Console.WriteLine("The given argument is not a valid filepath");
        }
    }

    private static void AddStructureToDatabaseIfNotExists(IDbConnection connection, DbContext context)
    {
        connection.Execute(
            CreateStatementManipulator.UpdateCreationScript(context.Database.GenerateCreateScript()));
    }

    public void Run(string command)
    {
        try
        {
            if (!string.IsNullOrEmpty(command))
            {
                Cli.Invoke(command);
                Console.Write($"{Environment.NewLine}$: ");
            }
        }
        catch (AggregateException e)
        {
            Console.Error.WriteLine(VerboseOutput ? e.ToString() : e.Message);

            foreach (var innerException in e.InnerExceptions)
                Console.Error.WriteLine(VerboseOutput ? innerException.ToString() : innerException.Message);
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

    private void ConsoleSetup()
    {
        Console.OutputEncoding = Encoding.UTF8;
    }

    private bool ConfigSetup(out IConfigManager? configManager)
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

    private bool IsValidConfig(IConfigManager configManager, string configFilePath)
    {
        if (configManager.GetEmptyKeys().Count == 0)
        {
            return true;
        }

        Console.WriteLine(
            $"Please fill {string.Join(", ", configManager.GetEmptyKeys())} in config file located at: {configFilePath}");
        return false;
    }
}