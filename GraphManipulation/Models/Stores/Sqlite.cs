using System.Data.SQLite;
using Dapper;
using GraphManipulation.Models.Structures;
using VDS.RDF;

namespace GraphManipulation.Models.Stores;

public class Sqlite : Relational
{
    private SQLiteConnection? _connection;

    public Sqlite(string name) : base(name)
    {
        DataStoreType = SupportedDataStores.Sqlite;
    }

    public Sqlite(string name, string baseUri) : this(name)
    {
        BaseUri = baseUri;
    }

    public Sqlite(string name, string baseUri, SQLiteConnection connection) : this(name, baseUri)
    {
        _connection = connection;
    }

    public void SetConnection(SQLiteConnection sqLiteConnection)
    {
        _connection = sqLiteConnection;
        ConnectionString = sqLiteConnection.ConnectionString;
    }

    public override string GetConnectionString()
    {
        if (_connection is null) throw new DataStoreException("Connection was null when building DataStore");

        return base.GetConnectionString();
    }

    public override void Build()
    {
        base.Build();

        if (_connection is null) throw new DataStoreException("Connection was null when building SQLite");

        _connection.Open();

        BuildSqlite();

        _connection.Close();
    }

    private void BuildSqlite()
    {
        Name = GetName();

        GetSchemas();
        GetTables();
        GetColumns();
        GetPrimaryKeys();
        GetForeignKeys();


        // var result = _connection
        //     .Query("SELECT * FROM sqlite_master AS m JOIN pragma_table_list(m.name);")
        //     .ToList();
        //
        // result.ForEach(Console.WriteLine);
        //
        // var command = _connection.CreateCommand();
        // command.CommandText = "SELECT tl.schema as schema_name, m.name as table_name, p.name as column_name, p.pk as primary_key " +
        //                       "FROM sqlite_master as m " +
        //                       "JOIN PRAGMA table_info m.name as p " +
        //                       "JOIN PRAGMA table_list m.name as tl " +
        //                       "ORDER BY m.name, p.cid";

        ComputeId();
    }

    private string GetName() => _connection.DataSource;

    private void GetSchemas() =>
        _connection
            .Query<string>("SELECT schema FROM sqlite_master JOIN pragma_table_list(sqlite_master.name);")
            .ToList()
            .ForEach(schema => AddStructure(new Schema(schema)));

    private void GetTables()
    {
        _connection
                .Query<(string, string)>(@"
                    SELECT tl.schema as schema_name, m.name as table_name
                    FROM sqlite_master as m
                    JOIN pragma_table_info(m.name) as p
                    JOIN pragma_table_list(m.name) as tl
                    ORDER BY m.name, p.cid;
                    ")
                .ToList()
                .ForEach(tuple =>
                    SubStructures
                        .Where(subStructure => tuple.Item1 == subStructure.Name)
                        .ToList()
                        .ForEach(sub => sub.AddStructure(new Table(tuple.Item2))));

        // foreach (var tuple in results)
        // {
        //     foreach (var subStructure in SubStructures.Where(subStructure => tuple.Item1 == subStructure.Name))
        //     {
        //         subStructure.AddStructure(new Table(tuple.Item2));
        //     }
        // }
    }

    private void GetColumns()
    {
    }

    private void GetPrimaryKeys()
    {
    }

    private void GetForeignKeys()
    {
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