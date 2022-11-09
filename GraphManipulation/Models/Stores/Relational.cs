using System.Data.Common;
using GraphManipulation.Extensions;
using GraphManipulation.Models.Structures;

namespace GraphManipulation.Models.Stores;

public abstract class Relational : Database
{
    protected List<ColumnOptionsQueryResult> ColumnOptionsQueryResults = new();
    protected List<ForeignKeysQueryResult> ForeignKeysQueryResults = new();

    protected List<StructureQueryResult> StructureQueryResults = new();

    protected Relational(string name) : base(name)
    {
    }

    protected Relational(string name, string baseUri) : base(name, baseUri)
    {
    }

    protected Relational(string name, string baseUri, DbConnection connection) : base(name, baseUri, connection)
    {
    }

    public override void BuildFromDataSource()
    {
        base.BuildFromDataSource();

        GetStructureQueryResults();
        GetForeignKeysQueryResults();
        GetColumnOptionsQueryResults();
        BuildStructure();
        BuildForeignKeys();
        BuildColumnOptions();
    }

    protected abstract void GetStructureQueryResults();

    protected abstract void GetForeignKeysQueryResults();

    protected abstract void GetColumnOptionsQueryResults();

    private void BuildStructure()
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

                    if (result.IsNotNull)
                    {
                        column.SetIsNotNull(true);
                    }
                });
            });
        });
    }

    private void BuildForeignKeys()
    {
        ForeignKeysQueryResults.ForEach(result =>
        {
            var fromTable = this
                .FindSchema(result.FromSchema)!
                .FindTable(result.FromTable)!;

            var fromColumn = fromTable.FindColumn(result.FromColumn)!;

            var toColumn = this
                .FindSchema(result.ToSchema)!
                .FindTable(result.ToTable)!
                .FindColumn(result.ToColumn)!;

            fromTable.AddForeignKey(new ForeignKey(fromColumn, toColumn, result.OnDelete, result.OnUpdate));
        });
    }

    private void BuildColumnOptions()
    {
        ColumnOptionsQueryResults.ForEach(result =>
        {
            this
                .FindSchema(result.SchemaName)!
                .FindTable(result.TableName)!
                .FindColumn(result.ColumnName)!
                .SetOptions(result.Options);
        });
    }

    public class StructureQueryResult
    {
        public StructureQueryResult(string schemaName, string tableName, string columnName,
            string dataType, long isPrimaryKey, long isNotNull)
        {
            SchemaName = schemaName;
            TableName = tableName;
            ColumnName = columnName;
            DataType = dataType;
            IsPrimaryKey = isPrimaryKey == 1;
            IsNotNull = isNotNull == 1;
        }

        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsNotNull { get; set; }
    }

    public class ForeignKeysQueryResult
    {
        public ForeignKeysQueryResult(
            string fromSchema, string fromTable, string fromColumn,
            string toSchema, string toTable, string toColumn,
            string onDelete, string onUpdate)
        {
            FromSchema = fromSchema;
            FromTable = fromTable;
            FromColumn = fromColumn;
            ToSchema = toSchema;
            ToTable = toTable;
            ToColumn = toColumn;

            OnDelete = onDelete.ToUpper() switch
            {
                "CASCADE" => ForeignKeyOnEnum.Cascade,
                "NO ACTION" => ForeignKeyOnEnum.NoAction,
                _ => throw new ForeignKeysQueryResultException("Action not supported: " + onDelete.ToUpper())
            };

            OnUpdate = onUpdate.ToUpper() switch
            {
                "CASCADE" => ForeignKeyOnEnum.Cascade,
                "NO ACTION" => ForeignKeyOnEnum.NoAction,
                _ => throw new ForeignKeysQueryResultException("Action not supported: " + onUpdate.ToUpper())
            };
        }

        public string FromSchema { get; set; }
        public string FromTable { get; set; }
        public string FromColumn { get; set; }
        public string ToSchema { get; set; }
        public string ToTable { get; set; }
        public string ToColumn { get; set; }
        public ForeignKeyOnEnum OnDelete { get; set; }
        public ForeignKeyOnEnum OnUpdate { get; set; }
    }

    public class ColumnOptionsQueryResult
    {
        public ColumnOptionsQueryResult(string schemaName, string tableName, string columnName, string options)
        {
            SchemaName = schemaName;
            TableName = tableName;
            ColumnName = columnName;
            Options = options;
        }

        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string Options { get; set; }
    }
}

public class ForeignKeysQueryResultException : Exception
{
    public ForeignKeysQueryResultException(string message) : base(message)
    {
    }
}