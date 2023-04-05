namespace GraphManipulation.DataAccess.Entities;

public class PersonDataColumn
{
    public PersonDataColumn(string tableName, string columnName, string defaultValue, IEnumerable<DeleteCondition> deleteConditions)
    {
        TableName = tableName;
        ColumnName = columnName;
        DefaultValue = defaultValue;
        DeleteConditions = deleteConditions;
    }

    public string TableName {get; set;}
    public string ColumnName {get; set;}
    public string DefaultValue {get; set;}
    public IEnumerable<DeleteCondition> DeleteConditions {get; set;}
}