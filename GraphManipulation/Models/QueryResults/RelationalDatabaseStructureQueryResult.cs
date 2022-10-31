namespace GraphManipulation.Models.QueryResults;

public class RelationalDatabaseStructureQueryResult
{
    public RelationalDatabaseStructureQueryResult(string schemaName, string tableName, string columnName, string dataType, long isPrimaryKey)
    {
        SchemaName = schemaName;
        TableName = tableName;
        ColumnName = columnName;
        DataType = dataType;
        IsPrimaryKey = isPrimaryKey == 1;
    }
    
    public string SchemaName { get; set; }
    public string TableName { get; set; }
    public string ColumnName { get; set; }
    public string DataType { get; set; }
    public bool IsPrimaryKey { get; set; }
}