using GraphManipulation.Models;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;

namespace GraphManipulation.Extensions;

public static class StructureNavigation
{
    public static Schema? FindSchema(this DataStore dataStore, string schemaName)
    {
        return dataStore.SubStructures.Select(sub => (sub as Schema)!).FirstOrDefault(s => s.Name == schemaName);
    }

    public static Table? FindTable(this Schema schema, string tableName)
    {
        return schema.SubStructures.Select(sub => (sub as Table)!).FirstOrDefault(s => s.Name == tableName);
    }

    public static Column? FindColumn(this Table table, string columnName)
    {
        return table.SubStructures.Select(sub => (sub as Column)!).FirstOrDefault(s => s.Name == columnName);
    }

    public static Column? FindPrimaryKey(this Table table, string primaryKeyName)
    {
        return table.PrimaryKeys.FirstOrDefault(s => s.Name == primaryKeyName);
    }

    public static ForeignKey? FindForeignKey(this Table table, string foreignKeyFromName)
    {
        return table.ForeignKeys.FirstOrDefault(s => s.From.Name == foreignKeyFromName);
    }
}