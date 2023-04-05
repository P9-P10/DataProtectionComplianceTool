﻿using GraphManipulation.Vacuuming.Components;

namespace GraphManipulation.Vacuuming;

public class Vacuumer : IVacuumer
{
    private readonly List<TableColumnPair> _tableColumnPairs;

    public Vacuumer(List<TableColumnPair> tableColumnPairs)
    {
        _tableColumnPairs = tableColumnPairs;
    }

    public List<string> GenerateUpdateStatement(string predefinedExpirationDate = "")
    {
        var outputQuery = new List<string>();
        foreach (var tcPair in _tableColumnPairs)
        {
            var query = $"UPDATE {tcPair.Table} SET {tcPair.Column} = {tcPair.UpdateValue} WHERE ";
            var logicOperator = " AND ";
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
        var place = inputString.LastIndexOf(occurrenceToReplace, StringComparison.Ordinal);

        return place == -1
            ? inputString
            : inputString.Remove(place, occurrenceToReplace.Length).Insert(place, replaceWith);
    }
}