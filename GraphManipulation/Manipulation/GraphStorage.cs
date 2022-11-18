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
        CREATE TABLE IF NOT EXISTS DatastoreGraphs (
           id INTEGER PRIMARY KEY AUTOINCREMENT,
           uri VARCHAR NOT NULL,
           datastoreType VARCHAR NOT NULL,
           from_date DATETIME DEFAULT(STRFTIME('%Y-%m-%d %H:%M:%f', 'NOW')),
           graph VARCHAR NOT NULL,
           operations VARCHAR
        );
    ";

    private const string SqlCreateStatementWithDrop =
        $"DROP TABLE IF EXISTS DatastoreGraphs;\n{SqlCreateStatement}";

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

    public void InitInsert(DataStore dataStore)
    {
        Insert(dataStore.Uri, dataStore.ToGraph(), new List<string>());
    }

    public void Insert(DataStore dataStore, IGraph graph, List<string> changes)
    {
        Insert(dataStore.Uri, graph, changes);
    }

    public void Insert(Uri datastoreUri, IGraph graph, List<string> changes)
    {
        if (!graph.ValidateUsing(_ontology).Conforms)
        {
            throw new GraphStorageException("Graph does not conform");
        }

        var datastoreType = graph.GetDataStoreDescriptionLanguageTypeFromUri(datastoreUri)!;

        var jsonChanges = JsonConvert.SerializeObject(changes);

        var insertStatement = $@"
            INSERT INTO DatastoreGraphs (uri, datastoreType, graph, operations) 
            VALUES ('{datastoreUri}', '{datastoreType}', '{graph.ToStorageString()}', '{string.Join(", ", jsonChanges)}')
        ";

        _dbConnection.Open();
        _dbConnection.Execute(insertStatement);
        _dbConnection.Close();
    }

    public IGraph GetLatest(DataStore dataStore)
    {
        return GetLatest(dataStore.Uri);
    }

    public IGraph GetLatest(Uri uri)
    {
        IGraph result = new Graph();

        var query = @$"
            SELECT graph 
            FROM DatastoreGraphs as ds 
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

    public List<(string, string)> GetListOfManagedDataStoresWithType()
    {
        const string query = "SELECT DISTINCT uri, datastoreType FROM DatastoreGraphs; ";

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