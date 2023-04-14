using System.Collections.Generic;
using System.Linq;
using Dapper;
using GraphManipulation.Vacuuming;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Test.Vacuuming;

public class SqliteVacuumerStoreTest
{
    private static IVacuumerStore InitiateVacuumerStore()
    {
        SqliteConnection sqliteConnection = new("Data Source=vacuumerTest.db");
        sqliteConnection.Execute("DROP TABLE IF EXISTS vacuuming_rules;");
        return new SqliteVacuumerStore(sqliteConnection);
    }

    [Fact]
    public void TestStoreVacuumingRule_Returns_Correct_Id()
    {
        IVacuumerStore sqLiteVacuumerStore = InitiateVacuumerStore();
        int result = sqLiteVacuumerStore.StoreVacuumingRule(new VacuumingRule("Rule", "Purpose", "Interval"));

        Assert.True(result == 1);
    }

    [Fact]
    public void TestFetchVacuumingRule_Returns_Correct_Value()
    {
        IVacuumerStore sqLiteVacuumerStore = InitiateVacuumerStore();
        VacuumingRule? insertedRule = new("Rule", "Purpose", "Interval");
        int resultId = sqLiteVacuumerStore.StoreVacuumingRule(insertedRule);

        VacuumingRule? result = sqLiteVacuumerStore.FetchVacuumingRule(resultId);

        Assert.Equal(insertedRule, result);
    }

    [Fact]
    public void TestFetchVacuumingRules_Returns_All_Rules()
    {
        IVacuumerStore sqLiteVacuumerStore = InitiateVacuumerStore();
        VacuumingRule insertedRule = new("Rule", "Purpose", "Interval");
        VacuumingRule secondRule = new("SecondRule", "SecondPurpose", "Interval");
        int resultId = sqLiteVacuumerStore.StoreVacuumingRule(insertedRule);
        sqLiteVacuumerStore.StoreVacuumingRule(secondRule);

        IEnumerable<VacuumingRule> result = sqLiteVacuumerStore.FetchVacuumingRules();

        List<VacuumingRule> vacuumingRules = result.ToList();
        Assert.Contains(insertedRule, vacuumingRules);
        Assert.Contains(secondRule, vacuumingRules);
        Assert.True(vacuumingRules.Count == 2);
    }
    
    [Fact]
    public void TestFetchVacuumingRules_Returns_All_EmptyList_If_No_Rules_Exists()
    {
        IVacuumerStore sqLiteVacuumerStore = InitiateVacuumerStore();

        IEnumerable<VacuumingRule> result = sqLiteVacuumerStore.FetchVacuumingRules();

        List<VacuumingRule> vacuumingRules = result.ToList();
        Assert.True(vacuumingRules.Count == 0);
        Assert.Empty(vacuumingRules);
    }

    [Fact]
    public void TestFetchVacuumingRule_Returns_Null_If_ID_Does_Not_Exist()
    {
        IVacuumerStore sqLiteVacuumerStore = InitiateVacuumerStore();

        VacuumingRule? result = sqLiteVacuumerStore.FetchVacuumingRule(1);

        Assert.Null(result);
    }

    [Fact]
    public void TestDeleteVacuumingRule()
    {
        IVacuumerStore sqLiteVacuumerStore = InitiateVacuumerStore();
        VacuumingRule? insertedRule = new("Rule", "Purpose", "Interval");
        int resultId = sqLiteVacuumerStore.StoreVacuumingRule(insertedRule);

        sqLiteVacuumerStore.DeleteVacuumingRule(resultId);

        VacuumingRule? result = sqLiteVacuumerStore.FetchVacuumingRule(resultId);

        Assert.Null(result);
    }

    [Fact]
    public void TestUpdateVacuumingRule()
    {
        IVacuumerStore sqLiteVacuumerStore = InitiateVacuumerStore();
        VacuumingRule? insertedRule = new("Rule", "Purpose", "Interval");
        int resultId = sqLiteVacuumerStore.StoreVacuumingRule(insertedRule);

        VacuumingRule newRule = new("NewName", "NewPurpose", "NewInterval");
        bool isExecuted = sqLiteVacuumerStore.UpdateVacuumingRule(resultId, newRule);

        VacuumingRule? result = sqLiteVacuumerStore.FetchVacuumingRule(resultId);

        Assert.Equal(result, newRule);
        Assert.True(isExecuted);
    }
}