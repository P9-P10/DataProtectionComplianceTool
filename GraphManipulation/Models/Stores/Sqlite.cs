using System.Data.Common;
using Dapper;
using VDS.RDF;

namespace GraphManipulation.Models.Stores;

public class Sqlite : Relational
{
    public Sqlite(string name) : base(name)
    {
        // DataStoreType = SupportedDataStores.Sqlite;
    }

    public Sqlite(string name, string baseUri) : base(name, baseUri)
    {
    }

    public Sqlite(string name, string baseUri, DbConnection connection) : base(name, baseUri, connection)
    {
        
    }

    protected override void GetStructureQueryResults()
    {
        StructureQueryResults = Connection
            .Query<StructureQueryResult>(@"
            SELECT tl.schema as schemaName, 
                   m.tbl_name as tableName, 
                   ti.name as columnName, 
                   ti.type as dataType, 
                   ti.pk as isPrimaryKey,
                   ti.'notnull' as isNotNull
            FROM sqlite_master as m
            JOIN pragma_table_info(m.tbl_name) as ti
            JOIN pragma_table_list(m.tbl_name) as tl
            WHERE m.type = 'table' AND m.tbl_name NOT LIKE 'sqlite_%';")
            .ToList();
    }

    protected override void GetForeignKeysQueryResults()
    {
        ForeignKeysQueryResults = Connection
            .Query<ForeignKeysQueryResult>(@"
            SELECT tl.schema as fromSchema, 
                   m.tbl_name as fromTable, 
                   fkl.'from' as fromColumn, 
                   fk.schema as toSchema, 
                   fkl.'table' as toTable, 
                   fkl.'to' as toColumn
            FROM sqlite_master as m
            JOIN pragma_foreign_key_list(m.tbl_name) as fkl
            JOIN pragma_table_list(m.tbl_name) as tl
            JOIN pragma_table_list(fkl.'table') as fk
            WHERE m.type = 'table' ;")
            .ToList();
    }

    public override void Build()
    {
        if (Connection is null) throw new DataStoreException("Connection was null when building SQLite");

        Connection.Open();

        Name = GetNameFromDataSource();

        base.Build();

        Connection.Close();

        ComputeId();
    }

    public override string ToCreateStatement()
    {
        throw new NotImplementedException();
    }

    public override void FromCreateStatement(string createStatement)
    {
        throw new NotImplementedException();
    }

    public override void GenerateInsertStatements()
    {
        throw new NotImplementedException();
    }

    private string GetNameFromDataSource() => Connection!.DataSource!;

    public override IGraph ToGraph()
    {
        var graph = base.ToGraph();
        return graph;
    }
}