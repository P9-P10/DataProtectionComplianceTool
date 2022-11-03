using GraphManipulation.Models;
using GraphManipulation.Models.Structures;

namespace GraphManipulation.Extensions;

public static class Transformations
{
    public static string ToSqlCreateStatement(this Entity entity)
    {
        switch (entity)
        {
            case Column column:
                return $"{column.Name} {column.DataType.ToUpper()}{(column.IsNotNull ? " NOT NULL" : "")}";
            
            case Table table:
                var subStructuresAsStrings = table.SubStructures.Select(ToSqlCreateStatement);
                var joinedSubStructures = string.Join(",\n\t", subStructuresAsStrings);
                var primaryKeys = $"PRIMARY KEY ({string.Join(", ", table.PrimaryKeys.Select(pk => pk.Name))})";

                var foreignKeys = string.Join(",\n\t", 
                    table.ForeignKeys
                        .GroupBy(fk => fk.To.ParentStructure)
                        .ToList()
                        .Select(parentGrouping =>
                {
                    var foreignKeysFrom = $"FOREIGN KEY ({string.Join(", ", table.ForeignKeys.Where(fk => fk.To.ParentStructure.Equals(parentGrouping.Key)).Select(fk => fk.From.Name))}) ";
                    var foreignKeysToTable = $"REFERENCES {parentGrouping.Key.Name} ";
                    var foreignKeysToColumns =
                        $"({string.Join(", ", parentGrouping.Select(fk => fk.To!.Name))})";

                    return foreignKeysFrom + foreignKeysToTable + foreignKeysToColumns;
                }));

                var definitions = string.Join(",\n\t",
                    new List<string> { joinedSubStructures, primaryKeys });

                definitions = string.IsNullOrEmpty(foreignKeys) ? definitions : $"{definitions},\n\t{foreignKeys}";
                
                return $"CREATE TABLE {table.Name} (\n\t{definitions}\n);";
            
            case StructuredEntity structuredEntity:
                return string.Join("\n", structuredEntity.SubStructures.Select(ToSqlCreateStatement));
            
            default:
                throw new SqlTransformationException("Type not supported: " + entity.GetType());
        }
    }
}

public class SqlTransformationException : Exception
{
    public SqlTransformationException(string message) : base(message)
    {
    }
}