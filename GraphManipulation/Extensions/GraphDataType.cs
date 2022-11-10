using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;
using GraphManipulation.Ontologies;

namespace GraphManipulation.Extensions;

public static class GraphDataType
{
    public static string GetGraphTypeString(this Entity entity)
    {
        return GetGraphTypeString(entity.GetType());
    }

    public static string GetGraphTypeString(Type type)
    {
        return type switch
        {
            { } when type == typeof(Column) => DataStoreDescriptionLanguage.Column,
            { } when type == typeof(Table) => DataStoreDescriptionLanguage.Table,
            { } when type == typeof(Schema) => DataStoreDescriptionLanguage.Schema,
            { } when type == typeof(Sqlite) => DataStoreDescriptionLanguage.Sqlite,
            _ => throw new GraphDataTypeException("Type not supported " + type)
        };
    }
}

public class GraphDataTypeException : Exception
{
    public GraphDataTypeException(string message) : base(message)
    {
    }
}