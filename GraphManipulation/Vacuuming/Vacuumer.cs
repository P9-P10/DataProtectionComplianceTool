using GraphManipulation.DataAccess.Entities;
using GraphManipulation.Services;

namespace GraphManipulation.Vacuuming;

public class Vacuumer : IVacuumer
{
    private readonly IEnumerable<PersonDataColumn> _personDataColumn;
    private readonly IQueryExecutor _queryExecutor;

    public Vacuumer(IPersonDataColumnService personDataColumnService, IQueryExecutor queryExecutor)
    {
        _queryExecutor = queryExecutor;
        _personDataColumn = personDataColumnService.GetColumns();
    }

    public List<DeletionExecution> GenerateUpdateStatement(string predefinedExpirationDate = "")
    {
        var executions = new List<DeletionExecution>();
        foreach (var personDataColumn in _personDataColumn)
        {
            var currentExecution = new DeletionExecution();
            var query =
                $"UPDATE {personDataColumn.TableName} SET {personDataColumn.ColumnName} = {personDataColumn.DefaultValue} WHERE ";
            var logicOperator = " AND ";
            foreach (var deleteCondition in personDataColumn.DeleteConditions)
            {
                query +=
                    $"({deleteCondition.Condition})";
                query += logicOperator;
                currentExecution.AddPurpose(deleteCondition.Purpose);
            }

            currentExecution.Column = personDataColumn.ColumnName;
            currentExecution.Table = personDataColumn.TableName;

            currentExecution.Query = ReplaceLastOccurrenceOfString(query, logicOperator);


            executions.Add(currentExecution);
        }

        return executions;
    }

    public List<DeletionExecution> Execute()
    {
        List<DeletionExecution> executions = GenerateUpdateStatement();
        foreach (var deletionExecution in executions)
        {
            _queryExecutor.Execute(deletionExecution.Query);
        }

        return executions;
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