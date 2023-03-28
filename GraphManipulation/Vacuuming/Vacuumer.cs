using System.Globalization;
using GraphManipulation.Vacuuming.Components;

namespace GraphManipulation.Vacuuming;

public class Vacuumer : IVacuumer
{
    private List<TableColumnPair> _tableColumnPairs;

    public Vacuumer(List<TableColumnPair> tableColumnPairs)
    {
        _tableColumnPairs = tableColumnPairs;
    }

    public string GenerateSqlQueryForDeletion(string predefinedExpirationDate = "")
    {
        string outputQuery = "";
        foreach (TableColumnPair tcPair in _tableColumnPairs)
        {
            string query;
            Purpose purpose = tcPair.GetPurposeWithOldestExpirationDate();
            string time = predefinedExpirationDate == "" ? purpose.GetExpirationDate : predefinedExpirationDate;
            if (time == DateTime.Now.ToString("yyyy-M-d h:m", CultureInfo.InvariantCulture))
            {
                query = $"SELECT {tcPair.Column} " +
                        $"FROM {tcPair.Table} " +
                        $"WHERE ({purpose.ExpirationCondition});";
            }
            else
            {
                query = $"SELECT {tcPair.Column}, expiration_date " +
                        $"FROM {tcPair.Table} " +
                        $"JOIN ({purpose.ExpirationCondition}) " +
                        $"WHERE expiration_date < {time} " +
                        $"AND uid = id;";
                
            }
            outputQuery += query + "\n";
        }

        return outputQuery;
    }
}