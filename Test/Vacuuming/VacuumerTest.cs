using System.Collections.Generic;
using System.Linq;
using GraphManipulation.Models;
using GraphManipulation.Vacuuming;
using Test.Vacuuming.TestClasses;
using Xunit;
using static Test.Vacuuming.VacuumingModelsMakers;

namespace Test.Vacuuming;

public class VacuumerTest
{
    private Vacuumer VacuumInstantiate(TestPersonalDataColumnMapper? personalDataColumnService = null,
        IQueryExecutor? queryExecutor = null)
    {
        personalDataColumnService ??= new TestPersonalDataColumnMapper();

        queryExecutor ??= new TestQueryExecutor();

        Vacuumer vacuumer = new(personalDataColumnService, queryExecutor);
        return vacuumer;
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
        TestPersonalDataColumnMapper? personalDataColumnService = new();
        Vacuumer vacuumer = VacuumInstantiate(personalDataColumnService);
        personalDataColumnService.Insert(PersonalDataColumnMaker());

        var query = vacuumer.GenerateUpdateStatement();

        DeletionExecution expected = DeletionExecutionMaker("UPDATE Table SET Column = 'Null' WHERE (Condition);");
        Assert.Contains(expected, query);
    }


    [Fact]
    public void TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_Multiple_TableColumnPairs_()
    {
        TestPersonalDataColumnMapper? personalDataColumnService = new();
        Vacuumer vacuumer = VacuumInstantiate(personalDataColumnService);
        personalDataColumnService.AddColumn(PersonalDataColumnMaker());
        personalDataColumnService.AddColumn(PersonalDataColumnMaker(tableName: "SecondTable", columnName: "SecondColumn"));


        var query = vacuumer.GenerateUpdateStatement();


        var deletionExecutions = query.ToList();
        DeletionExecution firstExpected = DeletionExecutionMaker("UPDATE Table SET Column = 'Null' WHERE (Condition);");
        DeletionExecution secondExpected =
            DeletionExecutionMaker("UPDATE SecondTable SET SecondColumn = 'Null' WHERE (Condition);",
                table: "SecondTable", column: "SecondColumn");
        Assert.Contains(firstExpected, deletionExecutions);
        Assert.Contains(secondExpected, deletionExecutions);
    }

    [Fact]
    public void TestExecuteExecutesCorrectly()
    {
        TestPersonalDataColumnMapper? personalDataColumnService = new();
        TestQueryExecutor testQueryExecutor = new();
        Vacuumer vacuumer = VacuumInstantiate(personalDataColumnService, testQueryExecutor);
        personalDataColumnService.AddColumn(PersonalDataColumnMaker());


        vacuumer.Execute();


        const string expectedQuery = "UPDATE Table SET Column = 'Null' WHERE (Condition);";
        Assert.Contains(expectedQuery, testQueryExecutor.Query);
    }

    [Fact]
    public void TestExecute_Executes_Correctly_With_Multiple_Executions()
    {
        TestPersonalDataColumnMapper? personalDataColumnService = new();
        TestQueryExecutor? testQueryExecutor = new();
        Vacuumer vacuumer = VacuumInstantiate(personalDataColumnService, testQueryExecutor);
        personalDataColumnService.AddColumn(PersonalDataColumnMaker());
        personalDataColumnService.AddColumn(PersonalDataColumnMaker(tableName: "SecondTable", columnName: "SecondColumn"));


        vacuumer.Execute();


        const string firstQuery = "UPDATE Table SET Column = 'Null' WHERE (Condition);";
        const string secondQuery = "UPDATE SecondTable SET SecondColumn = 'Null' WHERE (Condition);";
        Assert.Contains(firstQuery, testQueryExecutor.Query);
        Assert.Contains(secondQuery, testQueryExecutor.Query);
    }

    [Fact]
    public void TestExecute_Executes_Correctly_With_Multiple_Executions_Returns_Correct_DeletionExecutions()
    {
        TestPersonalDataColumnMapper? personalDataColumnService = new();
        TestQueryExecutor? testQueryExecutor = new();
        Vacuumer vacuumer = VacuumInstantiate(personalDataColumnService, testQueryExecutor);
        personalDataColumnService.AddColumn(PersonalDataColumnMaker());
        personalDataColumnService.AddColumn(PersonalDataColumnMaker(tableName: "SecondTable", columnName: "SecondColumn"));


        var query = vacuumer.Execute();


        var deletionExecutions = query.ToList();
        DeletionExecution firstExpected = DeletionExecutionMaker("UPDATE Table SET Column = 'Null' WHERE (Condition);");
        DeletionExecution secondExpected =
            DeletionExecutionMaker("UPDATE SecondTable SET SecondColumn = 'Null' WHERE (Condition);",
                table: "SecondTable", column: "SecondColumn");
        Assert.Contains(firstExpected, deletionExecutions);
        Assert.Contains(secondExpected, deletionExecutions);
    }

    [Fact]
    public void TestRunVacuumingRule_Executes_Correct_Execution()
    {
        TestPersonalDataColumnMapper? personalDataColumnMapper = new();
        personalDataColumnMapper.AddColumn(PersonalDataColumnMaker(purposeName: "Name"));
        Vacuumer vacuumer = VacuumInstantiate(personalDataColumnService: personalDataColumnMapper);
        VacuumingRule vacuumingRule = VacuumingRuleMaker("Name", "Description", "2d 5y");

        List<DeletionExecution> executions =
            vacuumer.ExecuteVacuumingRules(new List<VacuumingRule>() {vacuumingRule}).ToList();


        DeletionExecution expected = DeletionExecutionMaker("UPDATE Table SET Column = 'Null' WHERE (Condition);");

        Assert.Contains(expected, executions);
        Assert.True(1 == executions.Count);
    }

    [Fact]
    public void TestRunVacuumingRule_Executes_Correct_Executions_Different_Executions_With_Same_Purpose()
    {
        TestPersonalDataColumnMapper? personalDataColumnMapper = new();
        personalDataColumnMapper.AddColumn(PersonalDataColumnMaker(purposeName: "Name"));
        personalDataColumnMapper.AddColumn(PersonalDataColumnMaker(purposeName: "Name",
            columnName: "AnotherColumn"));
        Vacuumer vacuumer = VacuumInstantiate(personalDataColumnService: personalDataColumnMapper);
        VacuumingRule vacuumingRule = VacuumingRuleMaker("Name", "Description", "2d 5y");


        List<DeletionExecution> executions =
            vacuumer.ExecuteVacuumingRules(new List<VacuumingRule>() {vacuumingRule}).ToList();


        DeletionExecution expected = DeletionExecutionMaker("UPDATE Table SET Column = 'Null' WHERE (Condition);");
        DeletionExecution secondExpected =
            DeletionExecutionMaker("UPDATE Table SET AnotherColumn = 'Null' WHERE (Condition);", column: "AnotherColumn");

        Assert.Contains(expected, executions);
        Assert.Contains(secondExpected, executions);
        Assert.True(2 == executions.Count);
    }
}