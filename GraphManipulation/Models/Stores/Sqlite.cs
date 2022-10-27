using System.Data.SQLite;
using Dapper;

using VDS.RDF;

namespace GraphManipulation.Models.Stores;

public class Sqlite : Relational
{
    private SQLiteConnection? _connection;

    public Sqlite(string name) : base(name)
    {
        DataStoreType = SupportedDataStores.Sqlite;
    }

    public void SetConnection(SQLiteConnection sqLiteConnection)
    {
        _connection = sqLiteConnection;
    }

    public override string GetConnectionString()
    {
        string baseString = base.GetConnectionString();
        
        if (_connection is null) throw new DataStoreException("Connection was null when building DataStore");

        return _connection.ConnectionString;
    }

    public override void Build()
    {
        base.Build();
        BuildSqlite();
    }

    private void BuildSqlite()
    {
        if (_connection is null) throw new DataStoreException("Connection was null when building DataStore");
        
        _connection.Open();
        
        var result = _connection
            .Query("SELECT * FROM sqlite_master AS m JOIN pragma_table_list(m.name);")
            .ToList();

        result.ForEach(Console.WriteLine);
        
        var command = _connection.CreateCommand();
        // command.CommandText = "SELECT tl.schema as schema_name, m.name as table_name, p.name as column_name, p.pk as primary_key " +
        //                       "FROM sqlite_master as m " +
        //                       "JOIN PRAGMA table_info m.name as p " +
        //                       "JOIN PRAGMA table_list m.name as tl " +
        //                       "ORDER BY m.name, p.cid";

        _connection.Close();
    }

    public override IGraph ToGraph()
    {
        var graph = base.ToGraph();
        return graph;
    }

    protected override string GetGraphTypeString()
    {
        return "SQLite";
    }
}