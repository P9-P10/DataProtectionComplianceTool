using System.Reflection;

namespace GraphManipulation.Helpers;

public class InsertStatementBuilder
{
    public string Table { get; set; }
    public object InsertValues { get; set; }
    
    public InsertStatementBuilder(string table)
    {
        Table = table;
    }

    public string Build()
    {
        PropertyInfo[] props = InsertValues.GetType().GetProperties();

        List<string> columns = new List<string>();
        List<string> valuesAsStrings = new List<string>();
        foreach (PropertyInfo prop in props)
        {
            object value = prop.GetValue(InsertValues, null);
            if (value == null) continue; // Exclude properties that do not define values
            columns.Add(prop.Name);
            valuesAsStrings.Add(
                value is string ? 
                    $"'{value}'" // If the value is a string it must be surrounded by singlequotes
                    : value.ToString().ToLower()); // Otherwise it must be converted to a lowercase string. 
        }

        string valuesString =  string.Join(", ", valuesAsStrings);

        // Build string of the form (<column1>, <column2> ..., <column3>) 
        // The column names are defined by the property names of InsertValues
        string columnsString = $"({string.Join(", ", columns)})";
        
        return $"INSERT INTO {Table} {columnsString} VALUES({valuesString});";
    }
}