﻿using System.Data;
using Dapper;
using GraphManipulation.Vacuuming.Components;

namespace GraphManipulation.Vacuuming;

public class TableColumnPairParser
{
    private readonly IDbConnection _dbConnection;

    public TableColumnPairParser(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    /// <summary>
    ///     Fetches TableColumnPairs from the provided SQLConnection
    /// </summary>
    /// <returns></returns>
    public List<TableColumnPair> FetchTableColumnPairs()
    {
        var output = new List<TableColumnPair>();

        var queryResult =
            _dbConnection.Query<TableColumnPair>(
                "SELECT purpose, ttl, target_table, target_column, legally_required, origin, start_time " +
                "FROM gdpr_metadata " +
                "ORDER BY target_table,target_column;");

        var tableColumnPairs = queryResult.ToList();
        foreach (var tcPair in tableColumnPairs)
            if (output.Contains(tcPair))
            {
                var index = output.IndexOf(tcPair);
                output[index].AddPurposes(tcPair.GetPurposes);
            }
            else
            {
                output.Add(tcPair);
            }

        return output;
    }
}