using System.Collections.Generic;
using GraphManipulation.DataAccess.Entities;
using GraphManipulation.Services;
using GraphManipulation.Vacuuming;
using Xunit;

namespace Test.Vacuuming;

public class VacuumerTest
{
    private class TestPersonDataColumnService : IPersonDataColumnService
    {
        private readonly List<PersonDataColumn> _personDataColumns = new();

        public void AddColumn(PersonDataColumn inputColumn)
        {
            _personDataColumns.Add(inputColumn);
        }

        public IEnumerable<PersonDataColumn> GetColumns()
        {
            return _personDataColumns;
        }
    }

    private class TestQueryExecutor : IQueryExecutor
    {
        public void Execute(string query)
        {
        }
    }

    [Fact]
    public void TestGenerateSqlQueryForDeletion_Returns_Empty_Query_when_No_TablePairs_Provided()
    {
        TestPersonDataColumnService personDataColumnService = new();
        TestQueryExecutor testQueryExecutor = new();

        Vacuumer vacuumer = new(personDataColumnService, testQueryExecutor);
        var query = vacuumer.GenerateUpdateStatement();

        Assert.Empty(query);
    }

    [Fact]
    public void TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_TablePairs()
    {
        TestPersonDataColumnService personDataColumnService = new();
        TestQueryExecutor testQueryExecutor = new();
        DeleteCondition deleteCondition = new("Condition", "Purpose");
        List<DeleteCondition> deleteConditions = new List<DeleteCondition> {deleteCondition};
        PersonDataColumn personDataColumn = new("Table",
            "Column",
            "Null",
            deleteConditions);
        personDataColumnService.AddColumn(personDataColumn);


        Vacuumer vacuumer = new(personDataColumnService, testQueryExecutor);

        var query = vacuumer.GenerateUpdateStatement();


        List<string> purposes = new List<string>() {"Purpose"};
        const string expectedQuery = "UPDATE Table SET Column = Null WHERE (Condition);";
        DeletionExecution expected = new(purposes, "Column", "Table", expectedQuery);
        Assert.Contains(expected, query);
    }


    [Fact]
    public void
        TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_Multiple_Purposes_One_TableColumnPair()
    {
        TestPersonDataColumnService personDataColumnService = new();
        TestQueryExecutor testQueryExecutor = new();
        DeleteCondition deleteCondition = new("Condition", "Purpose");
        DeleteCondition deleteCondition2 = new DeleteCondition("SecondCondition", "Purpose");
        List<DeleteCondition> deleteConditions = new List<DeleteCondition> {deleteCondition, deleteCondition2};
        PersonDataColumn personDataColumn = new("Table",
            "Column",
            "Null",
            deleteConditions);
        personDataColumnService.AddColumn(personDataColumn);

        Vacuumer vacuumer = new(personDataColumnService, testQueryExecutor);
        var query = vacuumer.GenerateUpdateStatement();

        List<string> purposes = new List<string>() {"Purpose"};
        const string expectedQuery = "UPDATE Table SET Column = Null WHERE (Condition) AND (SecondCondition);";
        DeletionExecution expected = new(purposes, "Column", "Table", expectedQuery);

        Assert.Contains(expected, query);
    }


    [Fact]
    public void TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_Multiple_TableColumnPairs_()
    {
        TestPersonDataColumnService personDataColumnService = new();
        TestQueryExecutor testQueryExecutor = new();
        DeleteCondition deleteCondition = new("Condition", "Purpose");
        List<DeleteCondition> deleteConditions = new List<DeleteCondition> {deleteCondition};
        PersonDataColumn personDataColumn = new("Table",
            "Column",
            "Null",
            deleteConditions);
        PersonDataColumn personDataColumn2 = new("SecondTable", "SecondColumn", "Null", deleteConditions);
        personDataColumnService.AddColumn(personDataColumn);
        personDataColumnService.AddColumn(personDataColumn2);

        Vacuumer vacuumer = new(personDataColumnService, testQueryExecutor);
        var query = vacuumer.GenerateUpdateStatement();


        List<string> purposes = new List<string>() {"Purpose"};
        const string firstExpectedQuery = "UPDATE Table SET Column = Null WHERE (Condition);";
        DeletionExecution firstExpected = new(purposes, "Column", "Table", firstExpectedQuery);
        const string secondExpectedQuery = "UPDATE SecondTable SET SecondColumn = Null WHERE (Condition);";
        DeletionExecution secondExpected = new(purposes, "SecondColumn", "SecondTable", secondExpectedQuery);

        Assert.Contains(firstExpected, query);
        Assert.Contains(secondExpected, query);
    }


    [Fact]
    public void
        TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_TablePairs_With_Default_UpdateValue_Defined()
    {
        TestPersonDataColumnService personDataColumnService = new();
        TestQueryExecutor testQueryExecutor = new();
        DeleteCondition deleteCondition = new("Condition", "Purpose");
        List<DeleteCondition> deleteConditions = new List<DeleteCondition> {deleteCondition};
        PersonDataColumn personDataColumn = new("Table",
            "Column",
            "UpdateValue",
            deleteConditions);
        personDataColumnService.AddColumn(personDataColumn);

        Vacuumer vacuumer = new(personDataColumnService, testQueryExecutor);
        var query = vacuumer.GenerateUpdateStatement();


        List<string> purposes = new List<string>() {"Purpose"};
        const string expectedQuery = "UPDATE Table SET Column = UpdateValue WHERE (Condition);";
        DeletionExecution expected = new(purposes, "Column", "Table", expectedQuery);
        Assert.Contains(expected, query);
    }
}