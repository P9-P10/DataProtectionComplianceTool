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
            { } when type == typeof(Column) => DatabaseDescriptionLanguage.Column,
            { } when type == typeof(Table) => DatabaseDescriptionLanguage.Table,
            { } when type == typeof(Schema) => DatabaseDescriptionLanguage.Schema,
            { } when type == typeof(Sqlite) => DatabaseDescriptionLanguage.Sqlite,
            _ => throw new GraphDataTypeException("Type not supported " + type)
        };
    }

    public static Type GetTypeFromString(string typeString)
    {
        return typeString switch
        {
            DatabaseDescriptionLanguage.Column => typeof(Column),
            DatabaseDescriptionLanguage.Table => typeof(Table),
            DatabaseDescriptionLanguage.Schema => typeof(Schema),
            DatabaseDescriptionLanguage.Sqlite => typeof(Sqlite),
            _ => throw new GraphDataTypeException("Type string could not be converted to type: " + typeString)
        };
    }
}

public class GraphDataTypeException : Exception
{
    public GraphDataTypeException(string message) : base(message)
    {
    }
}