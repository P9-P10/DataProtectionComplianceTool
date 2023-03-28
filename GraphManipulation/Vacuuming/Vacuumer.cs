using System.Globalization;
using GraphManipulation.Vacuuming.Components;

namespace GraphManipulation.Vacuuming;

public class Vacuumer : IVacuumer
{
    private readonly List<TableColumnPair> _tableColumnPairs;

    public Vacuumer(List<TableColumnPair> tableColumnPairs)
    {
        _tableColumnPairs = tableColumnPairs;
    }

    public string GenerateSqlQueryForDeletion(string predefinedExpirationDate = "")
    {
        string outputQuery = string.Empty;
        foreach (TableColumnPair tcPair in _tableColumnPairs)
        {
            foreach (var purpose in tcPair.GetPurposes)
            {
                string query = $"SELECT {tcPair.Column} FROM {tcPair.Table} WHERE Exists({purpose.ExpirationCondition});";
                outputQuery += query;
            }
        }

        return outputQuery;
    }
}