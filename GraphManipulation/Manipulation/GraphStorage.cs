using System.Data.Common;
using System.Data.SQLite;
using Dapper;
using GraphManipulation.Extensions;
using GraphManipulation.Models.Stores;
using Newtonsoft.Json;
using VDS.RDF;

namespace GraphManipulation.Manipulation;

public class GraphStorage
{
    private const string SqlCreateStatement = @"
        CREATE TABLE IF NOT EXISTS Databases (
          uri VARCHAR PRIMARY KEY,
          databaseType VARCHAR NOT NULL,
          creationDate DATETIME DEFAULT(STRFTIME('%Y-%m-%d %H:%M:%f', 'NOW')) 
        );

        CREATE TABLE IF NOT EXISTS DatabaseGraphs (
           id INTEGER PRIMARY KEY AUTOINCREMENT,
           uri VARCHAR NOT NULL,
           from_date DATETIME DEFAULT(STRFTIME('%Y-%m-%d %H:%M:%f', 'NOW')),
           graph VARCHAR NOT NULL,
           operations VARCHAR NOT NULL,
           FOREIGN KEY (uri) REFERENCES Databases (uri) ON DELETE CASCADE
        );
    ";

    private const string SqlCreateStatementWithDrop =
        $"DROP TABLE IF EXISTS Databases; DROP TABLE IF EXISTS DatabaseGraphs;\n{SqlCreateStatement}";

    private readonly DbConnection _dbConnection;
    private readonly IGraph _ontology;

    public GraphStorage(string connectionString, IGraph ontology, bool withDrop = false)
    {
        _ontology = ontology;

        if (!File.Exists(connectionString))
        {
            SQLiteConnection.CreateFile(connectionString);
        }

        _dbConnection = new SQLiteConnection($"Data source={connectionString};Version=3;");
        _dbConnection.Open();
        if (withDrop)
        {
            _dbConnection.Execute(SqlCreateStatementWithDrop);
            Console.WriteLine("DROPPING TABLES");
        }
        else
        {
            _dbConnection.Execute(SqlCreateStatement);
        }

        _dbConnection.Close();
    }

    private void CheckConformity(IGraph graph)
    {
        if (!graph.ValidateUsing(_ontology).Conforms)
        {
            throw new GraphStorageException("Graph does not conform");
        }
    }

    public void InitInsert(Database database)
    {
        var graph = database.ToGraph();
        CheckConformity(graph);

        var databaseType = graph.GetDatabaseDescriptionLanguageTypeFromUri(database.Uri)!;

        var insertStatement = $@"
            INSERT INTO Databases (uri, databaseType) VALUES ('{database.Uri}', '{databaseType}')
        ";

        _dbConnection.Open();
        _dbConnection.Execute(insertStatement);
        _dbConnection.Close();

        Insert(database.Uri, graph, new List<string>());
    }

    public void Insert(Database database, IGraph graph, List<string> changes)
    {
        Insert(database.Uri, graph, changes);
    }

    public void Insert(Uri databaseUri, IGraph graph, List<string> changes)
    {
        CheckConformity(graph);

        var jsonChanges = JsonConvert.SerializeObject(changes);

        var insertStatement = $@"
            INSERT INTO DatabaseGraphs (uri, graph, operations) 
            VALUES ('{databaseUri}', '{graph.ToStorageString()}', '{string.Join(',', jsonChanges)}')
        ";

        _dbConnection.Open();
        _dbConnection.Execute(insertStatement);
        _dbConnection.Close();
    }

    public IGraph GetLatest(Database database)
    {
        return GetLatest(database.Uri);
    }

    public IGraph GetLatest(Uri uri)
    {
        IGraph result = new Graph();

        var query = @$"
            SELECT graph 
            FROM DatabaseGraphs as ds 
            WHERE ds.uri = '{uri}' 
            ORDER BY from_date 
            DESC LIMIT 1
        ";

        _dbConnection.Open();
        var queryResult = _dbConnection
            .Query<string>(query)
            .FirstOrDefault();
        _dbConnection.Close();

        if (queryResult is null)
        {
            throw new GraphStorageException("No graph found for uri: " + uri);
        }

        result.LoadFromString(queryResult);

        return result;
    }

    public List<(string, string)> GetListOfManagedDatabasesWithType()
    {
        const string query = "SELECT uri, databaseType FROM Databases; ";

        _dbConnection.Open();
        var result = _dbConnection.Query<(string, string)>(query).ToList();
        _dbConnection.Close();

        return result;
    }
}

public class GraphStorageException : Exception
{
    public GraphStorageException(string message) : base(message)
    {
    }
}