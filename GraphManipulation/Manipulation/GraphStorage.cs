using System.Data.Common;
using System.Data.SQLite;
using Dapper;
using GraphManipulation.Extensions;
using GraphManipulation.Models.Stores;
using VDS.RDF;

namespace GraphManipulation.Manipulation;

public class GraphStorage
{
    private const string ConnectionString = "/home/ane/Documents/GitHub/GraphManipulation/GraphManipulation/GraphStorage.sqlite";
    private DbConnection _dbConnection;
    private IGraph _ontology;

    private const string SqlCreateStatement = @"
        CREATE TABLE IF NOT EXISTS DatastoreGraphs (
           id INTEGER PRIMARY KEY AUTOINCREMENT,
           uri VARCHAR NOT NULL,
           from_date DATETIME DEFAULT CURRENT_TIMESTAMP,
           graph VARCHAR NOT NULL,
           operations VARCHAR
        );
    ";

    public GraphStorage(IGraph ontology)
    {
        _ontology = ontology;
        
        if (!File.Exists(ConnectionString))
        {
            SQLiteConnection.CreateFile(ConnectionString);
        }
        _dbConnection = new SQLiteConnection($"Data source={ConnectionString};Version=3;");
        _dbConnection.Open();
        _dbConnection.Execute(SqlCreateStatement);
        _dbConnection.Close();
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

        var insertStatement = $@"
            INSERT INTO DatastoreGraphs (uri, graph, operations) 
            VALUES ('{datastoreUri}', '{graph.ToStorageString()}', '{string.Join(", ", changes)}')
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
}

public class GraphStorageException : Exception
{
    public GraphStorageException(string message) : base(message)
    {
    }
}