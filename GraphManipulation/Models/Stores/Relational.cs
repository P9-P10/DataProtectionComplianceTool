using System.Data.Common;
using GraphManipulation.Models.Structures;

namespace GraphManipulation.Models.Stores;

public abstract class Relational : Database
{
    protected Relational(string name) : base(name)
    {
    }

    protected Relational(string name, string baseUri) : base(name, baseUri)
    {
        
    }

    protected Relational(string name, string baseUri, DbConnection connection) : base(name, baseUri, connection)
    {
        
    }

    protected List<StructureQueryResult> StructureQueryResults = new();

    protected List<ForeignKeysQueryResult> ForeignKeysQueryResults = new();

    public override void Build()
    {
        base.Build();
        
        GetStructureQueryResults();
        GetForeignKeysQueryResults();
        BuildStructure();
        BuildForeignKeys();
    }
    
    protected abstract void GetStructureQueryResults();

    protected abstract void GetForeignKeysQueryResults();

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
            var fromTable = 
                Table.GetTableFromSchema(result.FromTable, 
                    Schema.GetSchemaFromDatastore(result.FromSchema, 
                        this));
            
            var fromColumn = 
                Column.GetColumnFromTable(result.FromColumn, 
                    fromTable);
            
            var toColumn = 
                Column.GetColumnFromTable(result.ToColumn, 
                    Table.GetTableFromSchema(result.ToTable, 
                        Schema.GetSchemaFromDatastore(result.ToSchema, 
                            this)));
            
            fromTable.AddForeignKey(fromColumn, toColumn);
        });
    }
    
    public class StructureQueryResult
    {
        public StructureQueryResult(string schemaName, string tableName, string columnName, string dataType, long isPrimaryKey, long isNotNull)
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
        public ForeignKeysQueryResult(string fromSchema, string fromTable, string fromColumn, string toSchema, string toTable, string toColumn)
        {
            FromSchema = fromSchema;
            FromTable = fromTable;
            FromColumn = fromColumn;
            ToSchema = toSchema;
            ToTable = toTable;
            ToColumn = toColumn;
        }

        public string FromSchema { get; set; }
        public string FromTable { get; set; }
        public string FromColumn { get; set; }
        public string ToSchema { get; set; }
        public string ToTable { get; set; }
        public string ToColumn { get; set; }
    }
}