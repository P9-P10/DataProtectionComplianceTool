using System.Collections.Generic;
using System.Linq;
using GraphManipulation.DataAccess.Entities;
using GraphManipulation.Services;
using GraphManipulation.Vacuuming;
using Test.Vacuuming.TestClasses;
using Xunit;

namespace Test.Vacuuming;

public class VacuumerTest
{
    private Vacuumer VacuumInstantiate(IPersonDataColumnService? personDataColumnService = null,
        IQueryExecutor? queryExecutor = null, IVacuumerStore? vacuumerStore = null)
    {
        personDataColumnService ??= new TestPersonDataColumnService();

        queryExecutor ??= new TestQueryExecutor();

        vacuumerStore ??= new TestVacuumerStore();

        Vacuumer vacuumer = new(personDataColumnService, queryExecutor, vacuumerStore);
        return vacuumer;
    }

    private PersonDataColumn PersonDataColumnMaker(string defaultValue = "Null", bool multipleDeleteConditions = false,
        string tableName = "Table", string columnName = "Column", string purpose = "Purpose")
    {
        List<DeleteCondition> deleteConditions = new List<DeleteCondition>();
        if (!multipleDeleteConditions)
        {
            DeleteCondition deleteCondition = new("Condition", purpose);
            deleteConditions.Add(deleteCondition);
        }
        else
        {
            DeleteCondition deleteCondition = new("Condition", purpose);
            DeleteCondition deleteCondition2 = new("SecondCondition", purpose);
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
        TestPersonDataColumnService? personDataColumnService = new();
        Vacuumer vacuumer = VacuumInstantiate(personDataColumnService);
        personDataColumnService.AddColumn(PersonDataColumnMaker());

        var query = vacuumer.GenerateUpdateStatement();

        DeletionExecution expected = DeletionExecutionMaker("UPDATE Table SET Column = Null WHERE (Condition);");
        Assert.Contains(expected, query);
    }


    [Fact]
    public void
        TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_Multiple_Purposes_One_TableColumnPair()
    {
        TestPersonDataColumnService? personDataColumnService = new();
        Vacuumer vacuumer = VacuumInstantiate(personDataColumnService);
        personDataColumnService.AddColumn(PersonDataColumnMaker(multipleDeleteConditions: true));


        var query = vacuumer.GenerateUpdateStatement();


        DeletionExecution expected = DeletionExecutionMaker("UPDATE Table SET Column = Null " +
                                                            "WHERE (Condition) AND (SecondCondition);");
        Assert.Contains(expected, query);
    }


    [Fact]
    public void TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_Multiple_TableColumnPairs_()
    {
        TestPersonDataColumnService? personDataColumnService = new();
        Vacuumer vacuumer = VacuumInstantiate(personDataColumnService);
        personDataColumnService.AddColumn(PersonDataColumnMaker());
        personDataColumnService.AddColumn(PersonDataColumnMaker(tableName: "SecondTable", columnName: "SecondColumn"));


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
        TestPersonDataColumnService? personDataColumnService = new();
        TestQueryExecutor testQueryExecutor = new();
        Vacuumer vacuumer = VacuumInstantiate(personDataColumnService, testQueryExecutor);
        personDataColumnService.AddColumn(PersonDataColumnMaker());


        vacuumer.Execute();


        const string expectedQuery = "UPDATE Table SET Column = Null WHERE (Condition);";
        Assert.Contains(expectedQuery, testQueryExecutor.Query);
    }

    [Fact]
    public void TestExecute_Executes_Correctly_With_Multiple_Executions()
    {
        TestPersonDataColumnService? personDataColumnService = new();
        TestQueryExecutor? testQueryExecutor = new();
        Vacuumer vacuumer = VacuumInstantiate(personDataColumnService, testQueryExecutor);
        personDataColumnService.AddColumn(PersonDataColumnMaker());
        personDataColumnService.AddColumn(PersonDataColumnMaker(tableName: "SecondTable", columnName: "SecondColumn"));


        vacuumer.Execute();


        const string firstQuery = "UPDATE Table SET Column = Null WHERE (Condition);";
        const string secondQuery = "UPDATE SecondTable SET SecondColumn = Null WHERE (Condition);";
        Assert.Contains(firstQuery, testQueryExecutor.Query);
        Assert.Contains(secondQuery, testQueryExecutor.Query);
    }

    [Fact]
    public void TestExecute_Executes_Correctly_With_Multiple_Executions_Returns_Correct_DeletionExecutions()
    {
        TestPersonDataColumnService? personDataColumnService = new();
        TestQueryExecutor? testQueryExecutor = new();
        Vacuumer vacuumer = VacuumInstantiate(personDataColumnService, testQueryExecutor);
        personDataColumnService.AddColumn(PersonDataColumnMaker());
        personDataColumnService.AddColumn(PersonDataColumnMaker(tableName: "SecondTable", columnName: "SecondColumn"));


        var query = vacuumer.Execute();


        var deletionExecutions = query.ToList();
        DeletionExecution firstExpected = DeletionExecutionMaker("UPDATE Table SET Column = Null WHERE (Condition);");
        DeletionExecution secondExpected =
            DeletionExecutionMaker("UPDATE SecondTable SET SecondColumn = Null WHERE (Condition);",
                table: "SecondTable", column: "SecondColumn");
        Assert.Contains(firstExpected, deletionExecutions);
        Assert.Contains(secondExpected, deletionExecutions);
    }

    [Fact]
    public void TestAddVacuumingRule_Adds_Correct_Rule()
    {
        TestVacuumerStore vacuumerStore = new();
        Vacuumer vacuumer = VacuumInstantiate(vacuumerStore: vacuumerStore);

        int id = vacuumer.AddVacuumingRule("Rule", "Purpose", "2y 5d");

        VacuumingRule? rule = new("Rule", "Purpose", "2y 5d");
        Assert.True(vacuumerStore.FetchVacuumingRules().ToList().Count == 1);
        Assert.Contains(rule, vacuumerStore.FetchVacuumingRules());
        Assert.Equal(1, id);
    }

    [Fact]
    public void TestUpdateVacuumingRule_Updates_Values()
    {
        TestVacuumerStore vacuumerStore = new();
        Vacuumer vacuumer = VacuumInstantiate(vacuumerStore: vacuumerStore);
        int id = vacuumer.AddVacuumingRule("Rule", "Purpose", "2y 5d");

        vacuumer.UpdateVacuumingRule(id, "NewName", "NewPurpose", "2y 20d");


        VacuumingRule? expected = new("NewName", "NewPurpose", "2y 20d");
        VacuumingRule? oldUnexpected = new("Rule", "Purpose", "2y 5d");

        List<VacuumingRule?> storedRules = vacuumerStore.FetchVacuumingRules().ToList();
        Assert.DoesNotContain(oldUnexpected, storedRules);
        Assert.Contains(expected, storedRules);
        Assert.Single(storedRules);
    }

    [Fact]
    public void TestUpdateVacuumingRule_Only_Updates_Specified_Values()
    {
        TestVacuumerStore vacuumerStore = new();
        Vacuumer vacuumer = VacuumInstantiate(vacuumerStore: vacuumerStore);
        int id = vacuumer.AddVacuumingRule("Rule", "Purpose", "2y 5d");

        vacuumer.UpdateVacuumingRule(id, "NewName", newInterval: "2y 20d");


        VacuumingRule? expected = new("NewName", "Purpose", "2y 20d");
        VacuumingRule? oldUnexpected = new("Rule", "Purpose", "2y 5d");

        List<VacuumingRule?> storedRules = vacuumerStore.FetchVacuumingRules().ToList();
        Assert.DoesNotContain(oldUnexpected, storedRules);
        Assert.Contains(expected, storedRules);
    }

    [Fact]
    public void TestUpdateVacuumingRule_Updates_Values_Multiple_Rules()
    {
        TestVacuumerStore vacuumerStore = new();
        Vacuumer vacuumer = VacuumInstantiate(vacuumerStore: vacuumerStore);
        int id = vacuumer.AddVacuumingRule("Rule", "Purpose", "2y 5d");
        vacuumer.AddVacuumingRule("AnotherRule", "AnotherPurpose", "2y 5d");

        vacuumer.UpdateVacuumingRule(id, "NewName", "NewPurpose", "2y 20d");


        VacuumingRule? expected = new("NewName", "NewPurpose", "2y 20d");
        VacuumingRule? oldUnexpected = new("Rule", "Purpose", "2y 5d");


        List<VacuumingRule?> storedRules = vacuumerStore.FetchVacuumingRules().ToList();
        Assert.DoesNotContain(oldUnexpected, storedRules);
        Assert.Contains(expected, storedRules);
        Assert.Equal(2, storedRules.Count);
    }

    [Fact]
    public void TestDeleteVacuumingRule_Removes_Rule()
    {
        TestVacuumerStore vacuumerStore = new();
        Vacuumer vacuumer = VacuumInstantiate(vacuumerStore: vacuumerStore);
        int id = vacuumer.AddVacuumingRule("Rule", "Purpose", "2y 5d");

        vacuumer.DeleteVacuumingRule(id);


        List<VacuumingRule?> storedRules = vacuumerStore.FetchVacuumingRules().ToList();
        VacuumingRule? oldUnexpected = new("Rule", "Purpose", "2y 5d");
        Assert.DoesNotContain(oldUnexpected, storedRules);
        Assert.Empty(storedRules);
    }

    [Fact]
    public void TestRunVacuumingRule_Executes_Correct_Execution()
    {
        TestVacuumerStore vacuumerStore = new();
        TestQueryExecutor testExecutor = new();
        TestPersonDataColumnService personDataColumnService = new();
        personDataColumnService.AddColumn(PersonDataColumnMaker());
        personDataColumnService.AddColumn(PersonDataColumnMaker(purpose: "AnotherPurpose"));
        Vacuumer vacuumer = VacuumInstantiate(vacuumerStore: vacuumerStore, queryExecutor: testExecutor,
            personDataColumnService: personDataColumnService);
        int id = vacuumer.AddVacuumingRule("Rule", "Purpose", "2y 5d");

        List<DeletionExecution> executions = vacuumer.RunVacuumingRule(id).ToList();


        DeletionExecution expected = DeletionExecutionMaker("UPDATE Table SET Column = Null WHERE (Condition);");

        Assert.Contains(expected, executions);
        Assert.True(1 == executions.Count);
    }

    [Fact]
    public void TestRunVacuumingRule_Executes_Correct_Executions_Different_Executions_With_Same_Purpose()
    {
        TestVacuumerStore vacuumerStore = new();
        TestQueryExecutor testExecutor = new();
        TestPersonDataColumnService personDataColumnService = new();
        personDataColumnService.AddColumn(PersonDataColumnMaker());
        personDataColumnService.AddColumn(PersonDataColumnMaker(purpose: "Purpose",
            columnName: "AnotherColumn"));
        Vacuumer vacuumer = VacuumInstantiate(vacuumerStore: vacuumerStore, queryExecutor: testExecutor,
            personDataColumnService: personDataColumnService);
        int id = vacuumer.AddVacuumingRule("Rule", "Purpose", "2y 5d");

        List<DeletionExecution> executions = vacuumer.RunVacuumingRule(id).ToList();


        DeletionExecution expected = DeletionExecutionMaker("UPDATE Table SET Column = Null WHERE (Condition);");
        DeletionExecution secondExpected =
            DeletionExecutionMaker("UPDATE Table SET AnotherColumn = Null WHERE (Condition);",column:"AnotherColumn");

        Assert.Contains(expected, executions);
        Assert.True(2 == executions.Count);
    }
}