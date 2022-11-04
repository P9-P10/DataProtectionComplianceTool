using System.Data.Common;
using Dapper;
using GraphManipulation.Models.Structures;

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
                   fkl.'to' as toColumn,
                   fkl.on_delete as onDelete,
                   fkl.on_update as onUpdate
            FROM sqlite_master as m
            JOIN pragma_foreign_key_list(m.tbl_name) as fkl
            JOIN pragma_table_list(m.tbl_name) as tl
            JOIN pragma_table_list(fkl.'table') as fk
            WHERE m.type = 'table' ;")
            .ToList();
    }

    protected override void GetColumnOptionsQueryResults()
    {
        var result = Connection
            .Query<(string, string, string, string)>(@"
            SELECT tl.schema as schemaName, m.tbl_name as tableName, ti.name as columnName, sql
            FROM sqlite_master as m
            JOIN pragma_table_info(m.tbl_name) as ti
            JOIN pragma_table_list(m.tbl_name) as tl
            WHERE m.type = 'table' AND m.tbl_name NOT LIKE 'sqlite_%';");

        ColumnOptionsQueryResults = result.Select(tuple =>
        {
            var schemaName = tuple.Item1;
            var tableName = tuple.Item2;
            var columnName = tuple.Item3;
            var sql = tuple.Item4;

            var relevantLine = sql.Split("\n").Where(s => s.Contains(columnName));

            var options = sql
                .Split('\n')
                .Where(s => s.Contains(columnName))
                .Select(s => string.Join(" ", Column.ValidOptions.Where(s.ToUpper().Contains))).First().ToUpper();

            return new ColumnOptionsQueryResult(schemaName, tableName, columnName, options);
        }).ToList();
    }

    public override void BuildFromDataSource()
    {
        if (Connection is null)
        {
            throw new DataStoreException("Connection was null when building SQLite");
        }

        Connection.Open();

        Name = GetNameFromDataSource();

        base.BuildFromDataSource();

        Connection.Close();

        ComputeId();
    }

    private string GetNameFromDataSource()
    {
        return Connection!.DataSource!;
    }
}