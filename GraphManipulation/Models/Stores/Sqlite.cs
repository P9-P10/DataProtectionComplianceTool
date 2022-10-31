using System.Data.SQLite;
using Dapper;
using GraphManipulation.Models.QueryResults;
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
        // ConnectionString = sqLiteConnection.ConnectionString;
    }

    // public override string GetConnectionString()
    // {
    //     if (_connection is null) throw new DataStoreException("Connection was null when building DataStore");
    //
    //     return base.GetConnectionString();
    // }

    protected override void GetStructureQueryResults()
    {
        StructureQueryResults = _connection.Query<RelationalDatabaseStructureQueryResult>(@"
            SELECT tl.schema as schemaName, 
                   m.tbl_name as tableName, 
                   ti.name as columnName, 
                   ti.type as dataType, 
                   ti.pk as isPrimaryKey
            FROM sqlite_master as m
            JOIN pragma_table_info(m.tbl_name) as ti
            JOIN pragma_table_list(m.tbl_name) as tl
            WHERE m.type = 'table' AND m.tbl_name NOT LIKE 'sqlite_%';")
            .ToList();
    }

    public override void Build()
    {
        if (_connection is null) throw new DataStoreException("Connection was null when building SQLite");
        
        _connection.Open();
        
        Name = GetNameFromDataSource();
        
        base.Build();

        _connection.Close();
        
        ComputeId();
    }
    
    protected override void BuildForeignKeys()
    {
        
    }

    protected override void BuildStructure()
    {
        StructureQueryResults.GroupBy(r => r.SchemaName).ToList().ForEach(schemaGrouping =>
        {
            var schema = new Schema(schemaGrouping.Key);
            AddStructure(schema);

            schemaGrouping.GroupBy(r => r.TableName).ToList().ForEach(tableGrouping =>
            {
                var table = new Table(tableGrouping.Key);
                schema.AddStructure(table);
                
                tableGrouping.ToList().ForEach(result =>
                {
                    var column = new Column(result.ColumnName);
                    column.SetDataType(result.DataType);
                    
                    table.AddStructure(column);

                    if (result.IsPrimaryKey)
                    {
                        table.AddPrimaryKey(column);
                    }
                });
            });
        });
    }

    private string GetNameFromDataSource() => _connection.DataSource;

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