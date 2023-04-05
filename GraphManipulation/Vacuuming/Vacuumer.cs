using GraphManipulation.DataAccess.Entities;
using GraphManipulation.Services;

namespace GraphManipulation.Vacuuming;

public class Vacuumer : IVacuumer
{
    private IEnumerable<PersonDataColumn> _personDataColumn;

    public Vacuumer(IPersonDataColumnService personDataColumnService)
    {
        _personDataColumn = personDataColumnService.GetColumns();
    }

    public List<string> GenerateUpdateStatement(string predefinedExpirationDate = "")
    {
        var outputQuery = new List<string>();
        foreach (var personDataColumn in _personDataColumn)
        {
            var query = $"UPDATE {personDataColumn.TableName} SET {personDataColumn.ColumnName} = {personDataColumn.DefaultValue} WHERE ";
            var logicOperator = " AND ";
            foreach (var deleteCondition in personDataColumn.DeleteConditions)
            {
                query +=
                    $"({deleteCondition.Condition})";
                query += logicOperator;
            }

            outputQuery.Add(ReplaceLastOccurrenceOfString(query, logicOperator));
        }

        return outputQuery;
    }

    public List<DeletionExecution> Execute()
    {
        return new List<DeletionExecution>();
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