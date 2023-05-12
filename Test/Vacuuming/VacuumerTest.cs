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
    private Vacuumer VacuumInstantiate(TestPurposeMapper? personalDataColumnService = null,
        IQueryExecutor? queryExecutor = null)
    {
        personalDataColumnService ??= new TestPurposeMapper();

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
        TestPurposeMapper testPurposeMapper = new();
        Vacuumer vacuumer = VacuumInstantiate(testPurposeMapper);
        testPurposeMapper.Insert(PurposeMaker());

        var query = vacuumer.GenerateUpdateStatement();

        DeletionExecution expected = DeletionExecutionMaker("UPDATE Table SET Column = 'Null' WHERE (Condition);");
        Assert.Contains(expected, query);
    }


    [Fact]
    public void TestExecuteExecutesCorrectly()
    {
        TestPurposeMapper testPurposeMapper = new();
        TestQueryExecutor testQueryExecutor = new();
        Vacuumer vacuumer = VacuumInstantiate(testPurposeMapper, testQueryExecutor);
        testPurposeMapper.Insert(PurposeMaker());

        vacuumer.Execute();


        const string expectedQuery = "UPDATE Table SET Column = 'Null' WHERE (Condition);";
        Assert.Contains(expectedQuery, testQueryExecutor.Query);
    }

    [Fact]
    public void TestExecuteExecutesCorrectlyWithMultiplePurposesOnSameTable()
    {
        TestPurposeMapper testPurposeMapper = new();
        TestQueryExecutor testQueryExecutor = new();
        Vacuumer vacuumer = VacuumInstantiate(testPurposeMapper, testQueryExecutor);
        string firstName = "Name";
        string secondName = "NewName";
        testPurposeMapper.Insert(PurposeMaker(firstName));
        testPurposeMapper.Insert(PurposeMaker(secondName));

        List<DeletionExecution> conditions = vacuumer.Execute().ToList();
        const string expectedQuery = "UPDATE Table SET Column = 'Null' WHERE (Condition) AND (Condition);";

        Assert.Contains(expectedQuery, testQueryExecutor.Query);
        Assert.Contains(conditions, x => x.Purposes.Any(y => y.Name == firstName));
        Assert.Contains(conditions, x => x.Purposes.Any(y => y.Name == secondName));
    }

    [Fact]
    public void TestExecuteExecutesCorrectlyWithMultiplePurposesOnDifferentTables()
    {
        TestPurposeMapper testPurposeMapper = new();
        TestQueryExecutor testQueryExecutor = new();
        Vacuumer vacuumer = VacuumInstantiate(testPurposeMapper, testQueryExecutor);
        Purpose purpose = PurposeMaker();

        Purpose secondPurpose = PurposeMaker("AnotherName", tableName: "SecondTable", columnName: "SecondColumn",
            defaultValue: "NotNull", condition: "SecondCondition");
        testPurposeMapper.Insert(purpose);
        testPurposeMapper.Insert(secondPurpose);


        List<DeletionExecution> conditions = vacuumer.Execute().ToList();
        const string expectedQuery = "UPDATE Table SET Column = 'Null' WHERE (Condition);";
        const string secondExpectedQuery = "UPDATE SecondTable SET SecondColumn = 'NotNull' WHERE (SecondCondition);";
        Assert.Contains(expectedQuery, testQueryExecutor.Query);
        Assert.Contains(secondExpectedQuery, testQueryExecutor.Query);
        Assert.Contains(conditions, x => x.Purposes.Any(y => y.Name == purpose.GetName()));
        Assert.Contains(conditions, x => x.Purposes.Any(y => y.Name == secondPurpose.GetName()));
    }
    
    [Fact]
    public void TestExecuteExecutesCorrectlyOnePurposeMultipleDeletionConditions()
    {
        TestPurposeMapper testPurposeMapper = new();
        TestQueryExecutor testQueryExecutor = new();
        Vacuumer vacuumer = VacuumInstantiate(testPurposeMapper, testQueryExecutor);
        Purpose purpose = PurposeMaker();
        purpose.DeleteConditions = purpose.DeleteConditions.Append(DeleteConditionMaker(condition:"SecondCondition")).ToList();
        testPurposeMapper.Insert(purpose);
        

        List<DeletionExecution> conditions = vacuumer.Execute().ToList();
        const string expectedQuery = "UPDATE Table SET Column = 'Null' WHERE (Condition) AND (SecondCondition);";
        Assert.Contains(expectedQuery, testQueryExecutor.Query);
        Assert.Contains(conditions, x => x.Purposes.Any(y => y.Name == purpose.GetName()));

    }
}