using GraphManipulation.SchemaEvolution.Models;
using GraphManipulation.SchemaEvolution.Models.Entity;
using GraphManipulation.SchemaEvolution.Models.Stores;
using GraphManipulation.SchemaEvolution.Models.Structures;

namespace GraphManipulation.SchemaEvolution.Extensions;

public static class StructureNavigation
{
    public static T? Find<T>(this StructuredEntity structuredEntity, Uri uri) where T : Structure
    {
        if (structuredEntity.SubStructures.Count == 0)
        {
            return null;
        }

        var result = structuredEntity.SubStructures
            .FirstOrDefault(sub => sub.Uri == uri);

        if (result is T foundAtFirstLevel)
        {
            return foundAtFirstLevel;
        }

        return structuredEntity.SubStructures
            .Select(subStructure => subStructure.Find<T>(uri))
            .FirstOrDefault(found => found != null);
    }

    public static T? Find<T>(this StructuredEntity structuredEntity, Structure structure) where T : Structure
    {
        return structuredEntity.Find<T>(structure.Uri);
    }

    public static Schema? FindSchema(this Database database, string schemaName)
    {
        return database.SubStructures.Select(sub => (sub as Schema)!).FirstOrDefault(s => s.Name == schemaName);
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