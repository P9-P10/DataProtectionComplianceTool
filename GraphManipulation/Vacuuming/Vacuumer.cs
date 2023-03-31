﻿using GraphManipulation.Vacuuming.Components;

namespace GraphManipulation.Vacuuming;

public class Vacuumer : IVacuumer
{
    private readonly List<TableColumnPair> _tableColumnPairs;

    public Vacuumer(List<TableColumnPair> tableColumnPairs)
    {
        _tableColumnPairs = tableColumnPairs;
    }

    public List<string> GenerateSelectStatementForDataToDelete(string predefinedExpirationDate = "")
    {
        List<string> outputQuery = new List<string>();
        foreach (TableColumnPair tcPair in _tableColumnPairs)
        {
            string query = $"SELECT {tcPair.Column} FROM {tcPair.Table} WHERE ";
            string logicOperator = " AND ";
            foreach (var purpose in tcPair.GetPurposes)
            {
                query +=
                    $"({purpose.ExpirationCondition})";
                query += logicOperator;
            }

            outputQuery.Add(ReplaceLastOccurrenceOfString(query, logicOperator));
        }

        return outputQuery;
    }

    private string ReplaceLastOccurrenceOfString(string inputString, string occurrenceToReplace,
        string replaceWith = ";")
    {
        int place = inputString.LastIndexOf(occurrenceToReplace, StringComparison.Ordinal);

        return place == -1
            ? inputString
            : inputString.Remove(place, occurrenceToReplace.Length).Insert(place, replaceWith);
    }
}