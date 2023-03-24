using System.Data;
using GraphManipulation.Vacuuming.Components;

namespace GraphManipulation.Vacuuming;

public class Vacuumer : IVacuumer
{
    private IDbConnection _connection;
    private List<TableColumnPair> _tableColumnPairs;

    public Vacuumer(IDbConnection dbConnection, List<TableColumnPair> tableColumnPairs)
    {
        _connection = dbConnection;
        _tableColumnPairs = tableColumnPairs;
    }

    public bool DeleteExpiredTuples()
    {
        throw new NotImplementedException();
    }

    public string GenerateSqlQueryForDeletion(string predefinedExpirationDate = "")
    {
        string outputQuery = "";
        foreach (TableColumnPair tcPair in _tableColumnPairs)
        {
            Purpose purpose = tcPair.GetPurposeWithOldestExpirationDate();
            string time = predefinedExpirationDate == "" ? purpose.GetExpirationDate : predefinedExpirationDate;
            string query =
                $"SELECT {tcPair.Column}, expiration_date " +
                $"FROM {tcPair.Table} " +
                $"JOIN ({purpose.ExpirationCondition}) " +
                $"WHERE expiration_date < {time} " +
                $"AND uid = id;";
            outputQuery += query + "\n";
        }

        return outputQuery;
    }
}