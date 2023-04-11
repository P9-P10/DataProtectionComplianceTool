﻿using System.Collections.Generic;
using System.Linq;
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
        public List<string> Query;

        public TestQueryExecutor()
        {
            Query = new List<string>();
        }

        public void Execute(string query)
        {
            if (!Query.Contains(query))
            {
                Query.Add(query);
            }
        }
    }

    private Vacuumer VacuumInstantiate()
    {
        TestPersonDataColumnService personDataColumnService = new();
        TestQueryExecutor testQueryExecutor = new();

        Vacuumer vacuumer = new(personDataColumnService, testQueryExecutor);
        return vacuumer;
    }

    private PersonDataColumn PersonDataColumnMaker(string defaultValue = "Null", bool multipleDeleteConditions = false,
        string tableName = "Table", string columnName = "Column")
    {
        List<DeleteCondition> deleteConditions = new List<DeleteCondition>();
        if (!multipleDeleteConditions)
        {
            DeleteCondition deleteCondition = new("Condition", "Purpose");
            deleteConditions.Add(deleteCondition);
        }
        else
        {
            DeleteCondition deleteCondition = new("Condition", "Purpose");
            DeleteCondition deleteCondition2 = new("SecondCondition", "Purpose");
            deleteConditions.Add(deleteCondition);
            deleteConditions.Add(deleteCondition2);
        }

        PersonDataColumn personDataColumn = new(tableName,
            columnName,
            defaultValue,
            deleteConditions);
        return personDataColumn;
    }

    private DeletionExecution DeletionExecutionMaker(string query, string table = "Table", string column = "Column")
    {
        List<string> purposes = new List<string>() {"Purpose"};
        DeletionExecution deletionExecution = new(purposes, column, table, query);
        return deletionExecution;
    }

    [Fact]
    public void TestGenerateSqlQueryForDeletion_Returns_Empty_Query_when_No_TablePairs_Provided()
    {
        Vacuumer vacuumer = VacuumInstantiate();
        var query = vacuumer.GenerateUpdateStatement();

        Assert.Empty(query);
    }

    [Fact]
    public void TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_TablePairs()
    {
        TestPersonDataColumnService personDataColumnService = new();
        TestQueryExecutor testQueryExecutor = new();

        personDataColumnService.AddColumn(PersonDataColumnMaker());

        Vacuumer vacuumer = new(personDataColumnService, testQueryExecutor);

        var query = vacuumer.GenerateUpdateStatement();

        DeletionExecution expected = DeletionExecutionMaker("UPDATE Table SET Column = Null WHERE (Condition);");
        Assert.Contains(expected, query);
    }


    [Fact]
    public void
        TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_Multiple_Purposes_One_TableColumnPair()
    {
        TestPersonDataColumnService personDataColumnService = new();
        TestQueryExecutor testQueryExecutor = new();

        personDataColumnService.AddColumn(PersonDataColumnMaker(multipleDeleteConditions: true));

        Vacuumer vacuumer = new(personDataColumnService, testQueryExecutor);
        var query = vacuumer.GenerateUpdateStatement();

        DeletionExecution expected = DeletionExecutionMaker("UPDATE Table SET Column = Null " +
                                                            "WHERE (Condition) AND (SecondCondition);");

        Assert.Contains(expected, query);
    }


    [Fact]
    public void TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_Multiple_TableColumnPairs_()
    {
        TestPersonDataColumnService personDataColumnService = new();
        TestQueryExecutor testQueryExecutor = new();

        personDataColumnService.AddColumn(PersonDataColumnMaker());
        personDataColumnService.AddColumn(PersonDataColumnMaker(tableName: "SecondTable", columnName: "SecondColumn"));

        Vacuumer vacuumer = new(personDataColumnService, testQueryExecutor);
        var query = vacuumer.GenerateUpdateStatement();


        var deletionExecutions = query.ToList();
        DeletionExecution firstExpected = DeletionExecutionMaker("UPDATE Table SET Column = Null WHERE (Condition);");
        DeletionExecution secondExpected =
            DeletionExecutionMaker("UPDATE SecondTable SET SecondColumn = Null WHERE (Condition);",
                table: "SecondTable", column: "SecondColumn");
        Assert.Contains(firstExpected, deletionExecutions);
        Assert.Contains(secondExpected, deletionExecutions);
    }

    [Fact]
    public void TestExecuteExecutesCorrectly()
    {
        TestPersonDataColumnService personDataColumnService = new();
        TestQueryExecutor testQueryExecutor = new();

        personDataColumnService.AddColumn(PersonDataColumnMaker());

        Vacuumer vacuumer = new(personDataColumnService, testQueryExecutor);

        vacuumer.Execute();

        const string expectedQuery = "UPDATE Table SET Column = Null WHERE (Condition);";

        Assert.Contains(expectedQuery, testQueryExecutor.Query);
    }

    [Fact]
    public void TestExecute_Executes_Correctly_With_Multiple_Executions()
    {
        TestPersonDataColumnService personDataColumnService = new();
        TestQueryExecutor testQueryExecutor = new();

        personDataColumnService.AddColumn(PersonDataColumnMaker());
        personDataColumnService.AddColumn(PersonDataColumnMaker(tableName: "SecondTable", columnName: "SecondColumn"));

        Vacuumer vacuumer = new(personDataColumnService, testQueryExecutor);
        vacuumer.Execute();


        const string firstQuery = "UPDATE Table SET Column = Null WHERE (Condition);";
        const string secondQuery = "UPDATE SecondTable SET SecondColumn = Null WHERE (Condition);";

        Assert.Contains(firstQuery, testQueryExecutor.Query);
        Assert.Contains(secondQuery, testQueryExecutor.Query);
    }

    [Fact]
    public void TestExecute_Executes_Correctly_With_Multiple_Executions_Returns_Correct_DeletionExecutions()
    {
        TestPersonDataColumnService personDataColumnService = new();
        TestQueryExecutor testQueryExecutor = new();

        personDataColumnService.AddColumn(PersonDataColumnMaker());
        personDataColumnService.AddColumn(PersonDataColumnMaker(tableName: "SecondTable", columnName: "SecondColumn"));

        Vacuumer vacuumer = new(personDataColumnService, testQueryExecutor);
        var query = vacuumer.Execute();


        var deletionExecutions = query.ToList();
        DeletionExecution firstExpected = DeletionExecutionMaker("UPDATE Table SET Column = Null WHERE (Condition);");
        DeletionExecution secondExpected =
            DeletionExecutionMaker("UPDATE SecondTable SET SecondColumn = Null WHERE (Condition);",
                table: "SecondTable", column: "SecondColumn");
        Assert.Contains(firstExpected, deletionExecutions);
        Assert.Contains(secondExpected, deletionExecutions);
    }
}