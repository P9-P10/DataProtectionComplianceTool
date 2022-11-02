using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;

namespace GraphManipulation.Extensions;

public static class StructureNavigation
{
    // public static Column GetColumnFromTable(this Column column, string columnName, Table table)
    // {
    //     return (table.SubStructures.First(s => s.Name == columnName) as Column)!;
    // }

    

    public static Schema FindSchema(this DataStore dataStore, string schemaName)
    {
        return (dataStore.SubStructures.First(s => s.Name == schemaName) as Schema)!;
    }

    public static Table FindTable(this Schema schema, string tableName)
    {
        return (schema.SubStructures.First(s => s.Name == tableName) as Table)!;
    }

    public static Column FindColumn(this Table table, string columnName)
    {
        return (table.SubStructures.First(s => s.Name == columnName) as Column)!;
    }

    public static Column FindPrimaryKey(this Table table, string primaryKeyName)
    {
        return table.PrimaryKeys.First(s => s.Name == primaryKeyName);
    }

    public static Column FindForeignKey(this Table table, string foreignKeyName)
    {
        return table.ForeignKeys.First(s => s.Name == foreignKeyName);
    }
}