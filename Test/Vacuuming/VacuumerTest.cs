﻿using System.Collections.Generic;
using System.Linq;
using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Models;
using GraphManipulation.Services;
using GraphManipulation.Vacuuming;
using Test.Vacuuming.TestClasses;
using Xunit;
using static Test.Vacuuming.VacuumingModelsMakers;

namespace Test.Vacuuming;

public class VacuumerTest
{
    private Vacuumer VacuumInstantiate(IPersonDataColumnService? personDataColumnService = null,
        IQueryExecutor? queryExecutor = null, IMapper<VacuumingRule> vacuumingRuleMapper = null)
    {
        personDataColumnService ??= new TestPersonDataColumnService();

        queryExecutor ??= new TestQueryExecutor();

        vacuumingRuleMapper ??= new VacuumingRuleMapper();

        Vacuumer vacuumer = new(personDataColumnService, queryExecutor, vacuumingRuleMapper);
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
        TestPersonDataColumnService? personDataColumnService = new();
        Vacuumer vacuumer = VacuumInstantiate(personDataColumnService);
        personDataColumnService.AddColumn(VacuumingModelsMakers.PersonDataColumnMaker());

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
        TestVacuumingRuleMapper testVacuumingRuleMapper = new();
        Vacuumer vacuumer = VacuumInstantiate(vacuumingRuleMapper: testVacuumingRuleMapper);

        VacuumingRule actual = vacuumer.AddVacuumingRule("Rule", "2y 5d", "Description");

        VacuumingRule? expected = new(0, "Description", "Rule", "2y 5d", new List<Purpose>());
        Assert.True(testVacuumingRuleMapper.FetchVacuumingRules().ToList().Count == 1);
        Assert.Contains(expected, testVacuumingRuleMapper.FetchVacuumingRules());
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestUpdateVacuumingRule_Updates_Values()
    {
        TestVacuumingRuleMapper testVacuumingRuleMapper = new();
        Vacuumer vacuumer = VacuumInstantiate(vacuumingRuleMapper: testVacuumingRuleMapper);
        VacuumingRule oldRule = vacuumer.AddVacuumingRule("Rule", "Purpose", "2y 5d");

        vacuumer.UpdateVacuumingRule(oldRule,newName:"NewName",newDescription:"Description",newInterval:"2y 20d");


        VacuumingRule? expected = new(0,"Description", "NewName", "2y 20d");
        VacuumingRule? oldUnexpected = new(0,"Rule", "Purpose", "2y 5d");

        List<VacuumingRule> storedRules = testVacuumingRuleMapper.FetchVacuumingRules();
        Assert.DoesNotContain(oldUnexpected, storedRules);
        Assert.Contains(expected, storedRules);
        Assert.Single(storedRules);
    }

    [Fact]
    public void TestUpdateVacuumingRule_Only_Updates_Specified_Values()
    {
        TestVacuumingRuleMapper testVacuumingRuleMapper = new();
        Vacuumer vacuumer = VacuumInstantiate(vacuumingRuleMapper: testVacuumingRuleMapper);
        VacuumingRule oldVacuumingRule = vacuumer.AddVacuumingRule("Rule", "Purpose", "2y 5d");

        vacuumer.UpdateVacuumingRule(oldVacuumingRule, newDescription: "NewDescription", newInterval: "2y 20d",
            newName: "Purpose");


        VacuumingRule? expected = new(0, "NewDescription", "Purpose", "2y 20d");
        VacuumingRule? oldUnexpected = new(0, "Rule", "Purpose", "2y 5d");

        List<VacuumingRule> storedRules = testVacuumingRuleMapper.FetchVacuumingRules();
        Assert.DoesNotContain(oldUnexpected, storedRules);
        Assert.Contains(expected, storedRules);
    }

    [Fact]
    public void TestUpdateVacuumingRule_Updates_Values_Multiple_Rules()
    {
        TestVacuumingRuleMapper testVacuumingRuleMapper = new();
        Vacuumer vacuumer = VacuumInstantiate(vacuumingRuleMapper: testVacuumingRuleMapper);
        VacuumingRule oldVacuumingRule = vacuumer.AddVacuumingRule("Rule", "Purpose", "2y 5d");
        vacuumer.AddVacuumingRule("AnotherRule", "AnotherPurpose", "2y 5d");

        vacuumer.UpdateVacuumingRule(oldVacuumingRule, newDescription: "NewDescription", newInterval: "2y 20d",
            newName: "NewPurpose");


        VacuumingRule? expected = new(0,"NewDescription", "NewPurpose", "2y 20d");
        VacuumingRule? oldUnexpected = new(0,"Rule", "Purpose", "2y 5d");


        List<VacuumingRule> storedRules = testVacuumingRuleMapper.FetchVacuumingRules();
        Assert.DoesNotContain(oldUnexpected, storedRules);
        Assert.Contains(expected, storedRules);
        Assert.Equal(2, storedRules.Count);
    }

    [Fact]
    public void TestDeleteVacuumingRule_Removes_Rule()
    {
        TestVacuumingRuleMapper testVacuumingRuleMapper = new();
        Vacuumer vacuumer = VacuumInstantiate(vacuumingRuleMapper: testVacuumingRuleMapper);
        VacuumingRule oldVacuumingRule = vacuumer.AddVacuumingRule("Rule", "Purpose", "2y 5d");

        vacuumer.DeleteVacuumingRule(oldVacuumingRule);


        List<VacuumingRule> storedRules = testVacuumingRuleMapper.FetchVacuumingRules();
        VacuumingRule? oldUnexpected = new("Purpose", "Rule", "2y 5d");
        Assert.DoesNotContain(oldUnexpected, storedRules);
        Assert.Empty(storedRules);
    }

    [Fact]
    public void TestRunVacuumingRule_Executes_Correct_Execution()
    {
        Vacuumer vacuumer = VacuumInstantiate();
        VacuumingRule vacuumingRule = new("Name","Description", "2d 5y");

        List<DeletionExecution> executions = vacuumer.RunVacuumingRule(vacuumingRule).ToList();


        DeletionExecution expected = DeletionExecutionMaker("UPDATE Table SET Column = Null WHERE (Condition);");

        Assert.Contains(expected, executions);
        Assert.True(1 == executions.Count);
    }

    [Fact]
    public void TestRunVacuumingRule_Executes_Correct_Executions_Different_Executions_With_Same_Purpose()
    {
        TestVacuumingRuleMapper testVacuumingRuleMapper = new();
        TestQueryExecutor testExecutor = new();
        TestPersonDataColumnService personDataColumnService = new();
        personDataColumnService.AddColumn(PersonDataColumnMaker());
        personDataColumnService.AddColumn(PersonDataColumnMaker(purpose: "Purpose",
            columnName: "AnotherColumn"));
        Vacuumer vacuumer = VacuumInstantiate(vacuumingRuleMapper: testVacuumingRuleMapper, queryExecutor: testExecutor,
            personDataColumnService: personDataColumnService);
        Purpose purpose = new Purpose(identifier: 0, description: "Description", name: "Purpose");
        VacuumingRule vacuumingRule = vacuumer.AddVacuumingRule("Rule", "Purpose", "2y 5d");


        List<DeletionExecution> executions = vacuumer.RunVacuumingRule(vacuumingRule).ToList();


        DeletionExecution expected = DeletionExecutionMaker("UPDATE Table SET Column = Null WHERE (Condition);");
        DeletionExecution secondExpected =
            DeletionExecutionMaker("UPDATE Table SET AnotherColumn = Null WHERE (Condition);", column: "AnotherColumn");

        Assert.Contains(expected, executions);
        Assert.True(2 == executions.Count);
    }
}