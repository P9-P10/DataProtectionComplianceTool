using GraphManipulation.Models;
using GraphManipulation.Models.Stores;
using GraphManipulation.Models.Structures;

namespace GraphManipulation.Extensions;

public static class GraphDataType
{
    public static string GetGraphTypeString(this Entity entity)
    {
        switch (entity)
        {
            case Column:
                return "Column";
            case Table:
                return "Table";
            case Schema:
                return "Schema";
            case Sqlite:
                return "SQLite";
            default:
                throw new GraphDataTypeException("Type not supported " + entity.GetType());
        }
    }
}

public class GraphDataTypeException : Exception
{
    public GraphDataTypeException(string message) : base(message)
    {
    }
}