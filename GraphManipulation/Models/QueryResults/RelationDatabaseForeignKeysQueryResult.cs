namespace GraphManipulation.Models.QueryResults;

public class RelationDatabaseForeignKeysQueryResult
{
    public RelationDatabaseForeignKeysQueryResult(string fromSchema, string fromTable, string fromColumn, string toSchema, string toTable, string toColumn)
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