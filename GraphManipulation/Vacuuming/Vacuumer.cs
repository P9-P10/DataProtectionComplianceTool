using GraphManipulation.Vacuuming.Components;

namespace GraphManipulation.Vacuuming;

public class Vacuumer : IVacuumer
{
    private readonly List<TableColumnPair> _tableColumnPairs;

    public Vacuumer(List<TableColumnPair> tableColumnPairs)
    {
        _tableColumnPairs = tableColumnPairs;
    }

    public List<string> GenerateSqlQueryForDeletion(string predefinedExpirationDate = "")
    {
        List<string> outputQuery = new List<string>(); 
        foreach (TableColumnPair tcPair in _tableColumnPairs)
        {
            string query = $"SELECT {tcPair.Column} FROM {tcPair.Table} WHERE ";
            foreach (var purpose in tcPair.GetPurposes)
            {
                query +=
                    $"Exists({purpose.ExpirationCondition})";
                query += " OR ";
            }

            outputQuery.Add(ReplaceLastOr(query));
        }

        return outputQuery;
    }

    private string ReplaceLastOr(string inputString)
    {
        string orString = " OR ";
        int place = inputString.LastIndexOf(orString, StringComparison.Ordinal);

        return place == -1 ? inputString : inputString.Remove(place, orString.Length).Insert(place, ";");
    }
}