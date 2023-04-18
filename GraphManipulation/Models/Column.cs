namespace GraphManipulation.Models;

public class Column
{
    public Column(int id, string tableName, string columnName, IEnumerable<Purpose> purposes)
    {
        Id = id;
        TableName = tableName;
        ColumnName = columnName;
        Purposes = purposes;
    }

    public int Id { get; set; }
    public string TableName { get; set; }
    public string ColumnName { get; set; }
    public IEnumerable<Purpose> Purposes { get; set; }
}