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
                string time = predefinedExpirationDate == "" ? purpose.GetExpirationDate : predefinedExpirationDate;
                string query;
                if (time == DateTime.Now.ToString("yyyy-M-d h:m", CultureInfo.InvariantCulture))
                {
                    query = $"SELECT {tcPair.Column} " +
                            $"FROM {tcPair.Table} " +
                            $"WHERE ({purpose.ExpirationCondition})";
                }
                else
                {
                    query = $"SELECT {tcPair.Column}, expiration_date " +
                            $"FROM {tcPair.Table} " +
                            $"JOIN ({purpose.ExpirationCondition}) " +
                            $"WHERE expiration_date < {time} " +
                            $"AND uid = id";
                }

                outputQuery += query + " UNION ";
            }
        }

        return RemoveLastUnion(outputQuery);
    }

    private string RemoveLastUnion(string inputString)
    {
        int place = inputString.LastIndexOf(" UNION ", StringComparison.Ordinal);

        if (place == -1)
            return inputString;

        return inputString.Remove(place, " UNION ".Length).Insert(place, ";");
    }
}