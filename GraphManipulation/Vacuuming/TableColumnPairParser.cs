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
        var queryResult =
            _dbConnection.Query<TableColumnPair>(
                "SELECT purpose, ttl, target_table, target_column, legally_required, origin, start_time " +
                "FROM gdpr_metadata " +
                "ORDER BY target_table,target_column;");

        foreach (var row in queryResult)
        {
            
        }
        
        return new List<TableColumnPair>();
    }

    public List<TableColumnPair> ParsePurposes()
    {
        return new List<TableColumnPair>();
    }
}