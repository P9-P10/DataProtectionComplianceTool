using System.Data;
using Dapper;
using GraphManipulation.Vacuuming.Components;

namespace GraphManipulation.Vacuuming;

public class TableColumnPairParser
{
    private IDbConnection _dbConnection;

    public TableColumnPairParser(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public List<TableColumnPair> FetchPurposes()
    {
        List<TableColumnPair> output = new List<TableColumnPair>();
        
        var queryResult =
            _dbConnection.Query<TableColumnPair>(
                "SELECT purpose, ttl, target_table, target_column, legally_required, origin, start_time " +
                "FROM gdpr_metadata " +
                "ORDER BY target_table,target_column;");

        var tableColumnPairs = queryResult.ToList();
        foreach (TableColumnPair? tcpair in tableColumnPairs)
        {
            if (output.Contains(tcpair))
            {
                var index = output.IndexOf(tcpair);
                output[index].AddPurposes(tcpair.GetPurposes);
            }
            else
            {
                output.Add(tcpair);
            }
        }

        return output;
    }
}