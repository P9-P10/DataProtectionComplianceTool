// See https://aka.ms/new-console-template for more information

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

public static class Program
{
    private static string configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
    private static readonly bool VerboseOutput = false;

    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            if (args.Length == 1)
            {
                configPath = args[0];
            }
            else
            {
                Console.WriteLine(
                    "Received too many arguments. Only a single argument specifying the path of the configuration file expected");
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
        var purposeMapper = new Mapper<Purpose>(context);

        var vacuumer = new Vacuumer(purposeMapper, new SqliteQueryExecutor(dbConnection));

        IManagerFactory managerFactory = new LoggingManagerFactory(new ManagerFactory(context), logger);
        ILoggerFactory loggerFactory = new PlaintextLoggerFactory(logger);
        IVacuumerFactory vacuumerFactory = new LoggingVacuumerFactory(new VacuumerFactory(vacuumer), logger);

        var commandLineInterface = new CommandLineInterface(managerFactory, loggerFactory, vacuumerFactory);

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
            try
            {
                Console.Write($"{Environment.NewLine}{CommandLineInterface.Prompt} ");
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

    private static void ConsoleSetup()
    {
        Console.OutputEncoding = Encoding.UTF8;
    }

    private static bool ConfigSetup(out IConfigManager? configManager)
    {
        var configFilePath = configPath;
        var configValues = new Dictionary<string, string>
        {
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
        if (configManager.GetEmptyKeys().Count == 0)
        {
            return true;
        }

        Console.WriteLine(
            $"Please fill {string.Join(", ", configManager.GetEmptyKeys())} in config file located at: {configFilePath}");
        return false;
    }
}