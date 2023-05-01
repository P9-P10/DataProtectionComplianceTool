using System.Collections.Generic;
using System.Linq;
using GraphManipulation.DataAccess;
using GraphManipulation.DataAccess.Mappers;
using GraphManipulation.Models;
using GraphManipulation.Vacuuming;
using Test.Vacuuming.TestClasses;
using Xunit;
using static Test.Vacuuming.VacuumingModelsMakers;

namespace Test.Vacuuming;

public class VacuumerTest
{
    private Vacuumer VacuumInstantiate(TestPersonalDataColumnMapper? personDataColumnService = null,
        IQueryExecutor? queryExecutor = null, IMapper<VacuumingRule> vacuumingRuleMapper = null)
    {
        personDataColumnService ??= new TestPersonalDataColumnMapper();

        queryExecutor ??= new TestQueryExecutor();

        vacuumingRuleMapper ??= new Mapper<VacuumingRule>(new GdprMetadataContext(""));

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
        TestPersonalDataColumnMapper? personDataColumnService = new();
        Vacuumer vacuumer = VacuumInstantiate(personDataColumnService);
        personDataColumnService.Insert(PersonDataColumnMaker());

        var query = vacuumer.GenerateUpdateStatement();

        DeletionExecution expected = DeletionExecutionMaker("UPDATE Table SET Column = Null WHERE (Condition);");
        Assert.Contains(expected, query);
    }


    [Fact]
    public void TestGenerateSqlQueryForDeletion_Returns_Correct_Query_When_Provided_Multiple_TableColumnPairs_()
    {
        TestPersonalDataColumnMapper? personDataColumnService = new();
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
        TestPersonalDataColumnMapper? personDataColumnService = new();
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
        TestPersonalDataColumnMapper? personDataColumnService = new();
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
        TestPersonalDataColumnMapper? personDataColumnService = new();
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

        vacuumer.UpdateVacuumingRule(oldRule, newName: "NewName", newDescription: "Description", newInterval: "2y 20d");


        VacuumingRule? expected = new(0, "Description", "NewName", "2y 20d");
        VacuumingRule? oldUnexpected = new(0, "Rule", "Purpose", "2y 5d");

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


        VacuumingRule? expected = new(0, "NewDescription", "NewPurpose", "2y 20d");
        VacuumingRule? oldUnexpected = new(0, "Rule", "Purpose", "2y 5d");


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
    public void TestGetAllVacuumingRules_Returns_Correct_values_Single_Element_In_List()
    {
        TestVacuumingRuleMapper testVacuumingRuleMapper = new();
        Vacuumer vacuumer = VacuumInstantiate(vacuumingRuleMapper: testVacuumingRuleMapper);
        VacuumingRule rule = vacuumer.AddVacuumingRule("Rule", "Purpose", "2y 5d");

        List<VacuumingRule> rules = vacuumer.GetAllVacuumingRules().ToList();


        Assert.Contains(rule, rules);
        Assert.Single(rules);
    }

    [Fact]
    public void TestGetAllVacuumingRules_Returns_Correct_values_Multiple_Elements_In_List()
    {
        TestVacuumingRuleMapper testVacuumingRuleMapper = new();
        Vacuumer vacuumer = VacuumInstantiate(vacuumingRuleMapper: testVacuumingRuleMapper);
        VacuumingRule rule = vacuumer.AddVacuumingRule("Rule", "Purpose", "2y 5d");
        VacuumingRule secondRule = vacuumer.AddVacuumingRule("SecondRUle", "SecondInterval", "Description");

        List<VacuumingRule> rules = vacuumer.GetAllVacuumingRules().ToList();


        Assert.Contains(rule, rules);
        Assert.Contains(secondRule, rules);
    }

    [Fact]
    public void TestGetVacuumingRule_Returns_Correct_Value()
    {
        TestVacuumingRuleMapper testVacuumingRuleMapper = new();
        Vacuumer vacuumer = VacuumInstantiate(vacuumingRuleMapper: testVacuumingRuleMapper);
        VacuumingRule rule = vacuumer.AddVacuumingRule("Rule", "Purpose", "2y 5d");

        VacuumingRule foundRule = vacuumer.GetVacuumingRule(0);


        Assert.Equal(rule, foundRule);
    }

    [Fact]
    public void TestRunVacuumingRule_Executes_Correct_Execution()
    {
        TestPersonalDataColumnMapper? personalDataColumnMapper = new();
        personalDataColumnMapper.AddColumn(PersonDataColumnMaker(purposeName: "Name"));
        Vacuumer vacuumer = VacuumInstantiate(personDataColumnService: personalDataColumnMapper);
        VacuumingRule vacuumingRule = VacuumingRuleMaker("Name", "Description", "2d 5y");

        List<DeletionExecution> executions =
            vacuumer.RunVacuumingRules(new List<VacuumingRule>() {vacuumingRule}).ToList();


        DeletionExecution expected = DeletionExecutionMaker("UPDATE Table SET Column = Null WHERE (Condition);");

        Assert.Contains(expected, executions);
        Assert.True(1 == executions.Count);
    }

    [Fact]
    public void TestRunVacuumingRule_Executes_Correct_Executions_Different_Executions_With_Same_Purpose()
    {
        TestPersonalDataColumnMapper? personalDataColumnMapper = new();
        personalDataColumnMapper.AddColumn(PersonDataColumnMaker(purposeName: "Name"));
        personalDataColumnMapper.AddColumn(PersonDataColumnMaker(purposeName: "Name",
            columnName: "AnotherColumn"));
        Vacuumer vacuumer = VacuumInstantiate(personDataColumnService: personalDataColumnMapper);
        VacuumingRule vacuumingRule = VacuumingRuleMaker("Name", "Description", "2d 5y");


        List<DeletionExecution> executions =
            vacuumer.RunVacuumingRules(new List<VacuumingRule>() {vacuumingRule}).ToList();


        DeletionExecution expected = DeletionExecutionMaker("UPDATE Table SET Column = Null WHERE (Condition);");
        DeletionExecution secondExpected =
            DeletionExecutionMaker("UPDATE Table SET AnotherColumn = Null WHERE (Condition);", column: "AnotherColumn");

        Assert.Contains(expected, executions);
        Assert.Contains(secondExpected, executions);
        Assert.True(2 == executions.Count);
    }
}