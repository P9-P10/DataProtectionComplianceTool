using System.Xml;
using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;

namespace GraphManipulation.Extensions;

public static class GraphDataType
{
    public static string GetGraphTypeString(this Entity entity)
    {
        return GetGraphTypeString(entity.GetType());
    }

    public static string GetGraphTypeString(Type type)
    {
        const string prefix = "ddl:";

        return prefix + type switch
        {
            { } when type == typeof(Column) => "Column",
            { } when type == typeof(Table) => "Table",
            { } when type == typeof(Schema) => "Schema",
            { } when type == typeof(Sqlite) => "SQLite",
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