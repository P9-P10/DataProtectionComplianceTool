using GraphManipulation.Models.Entity;
using GraphManipulation.Models.Structures;

namespace GraphManipulation.Extensions;

public static class DatabaseToSqlCreateStatement
{
    public static string ToSqlCreateStatement(this Entity entity)
    {
        switch (entity)
        {
            case Column column:
                var name = column.Name;
                var dataType = column.DataType.ToUpper();
                var isNotNull = column.IsNotNull ? "NOT NULL" : "";
                var options = !string.IsNullOrEmpty(column.Options) ? column.Options : "";

                var dataTypeOptionsSpacing = !string.IsNullOrEmpty(column.Options) ? " " : "";
                var optionsIsNotNullSpacing = column.IsNotNull ? " " : "";

                return $"{name} {dataType}{dataTypeOptionsSpacing}{options}{optionsIsNotNullSpacing}{isNotNull}";

            case Table table:
                var subStructuresAsStrings = table.SubStructures.Select(ToSqlCreateStatement);
                var joinedSubStructures = string.Join(",\n\t", subStructuresAsStrings);
                var primaryKeys = $"PRIMARY KEY ({string.Join(", ", table.PrimaryKeys.Select(pk => pk.Name))})";

                var foreignKeys = string.Join(",\n\t",
                    table.ForeignKeys
                        .GroupBy(fk => fk.To.ParentStructure)
                        .Select(parentGrouping =>
                        {
                            var joinedForeignKeyNames = string.Join(", ",
                                table.ForeignKeys
                                    .Where(fk => fk.To.ParentStructure!.Equals(parentGrouping.Key))
                                    .Select(fk => fk.From.Name));
                            var foreignKeysFrom = $"FOREIGN KEY ({joinedForeignKeyNames}) ";
                            var foreignKeysToTable = $"REFERENCES {parentGrouping.Key!.Name} ";
                            var foreignKeysToColumns =
                                $"({string.Join(", ", parentGrouping.Select(fk => fk.To.Name))}) ";

                            var foreignKeyOnActions =
                                $"ON DELETE {parentGrouping.First().OnDeleteString} ON UPDATE {parentGrouping.First().OnUpdateString}";

                            return foreignKeysFrom + foreignKeysToTable + foreignKeysToColumns + foreignKeyOnActions;
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