namespace GraphManipulation.Helpers;

public class InsertStatementBuilder
{
    public InsertStatementBuilder(string table)
    {
        Table = table;
    }

    public string Table { get; set; }
    public object InsertValues { get; set; }

    public string Build()
    {
        var props = InsertValues.GetType().GetProperties();

        var columns = new List<string>();
        var valuesAsStrings = new List<string>();
        foreach (var prop in props)
        {
            var value = prop.GetValue(InsertValues, null);
            if (value == null)
            {
                continue; // Exclude properties that do not define values
            }

            columns.Add(prop.Name);
            valuesAsStrings.Add(
                value is string
                    ? $"'{value}'" // If the value is a string it must be surrounded by singlequotes
                    : value.ToString().ToLower()); // Otherwise it must be converted to a lowercase string. 
        }

        var valuesString = string.Join(", ", valuesAsStrings);

        // Build string of the form (<column1>, <column2> ..., <column3>) 
        // The column names are defined by the property names of InsertValues
        var columnsString = $"({string.Join(", ", columns)})";

        return $"INSERT INTO {Table} {columnsString} VALUES({valuesString});";
    }
}