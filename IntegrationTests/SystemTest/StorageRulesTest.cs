using FluentAssertions;
using GraphManipulation.Models;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class StorageRulesTest : TestResources
{
    [Fact]
    public void TestAddCommand_Returns_Correct_Message()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStorageRule(process, new StorageRule() { Key = "DeletionCondition", VacuumingCondition = "Condition" });
        var result = process.GetAllOutput();

        result.Should().ContainSingle(s =>
            s.Contains("Successfully") && s.Contains("created") && s.Contains("DeletionCondition"));
    }

    [Fact]
    public void TestAddCommand_Stores_Value()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStorageRule(process, new StorageRule() { Key = "DeletionCondition", VacuumingCondition = "Condition" });
        ListStorageRules(process);
        string result = process.GetOutput();

        result.Should().Contain("DeletionCondition, , Condition");
    }


    [Fact]
    public void TestUpdateCommand_Updates_Name_Updated_Value_Is_Stored()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();
        StorageRule storageRule = new() { Key = "DeletionCondition", VacuumingCondition = "Condition" };
        AddStorageRule(process, storageRule);


        UpdateStorageRule(process, storageRule,
            new StorageRule() { Key = "NewName", VacuumingCondition = Condition });

        ListStorageRules(process);
        List<string> result = process.GetLastOutput();
        result.Should().ContainSingle(s => s.Contains("NewName, , TRUE"));
    }

    [Fact]
    public void TestUpdateCommand_Updates_Description_Updated_Value_Is_Stored()
    {
        using TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();
        StorageRule storageRule = new() { Key = "DeletionCondition", VacuumingCondition = "Condition" };
        AddStorageRule(process, storageRule);


        UpdateStorageRule(process, storageRule, new StorageRule()
        {
            Key = "NewName", VacuumingCondition = "Condition",
            Description = "This is the new description"
        });
        ListStorageRules(process);

        List<string> result = process.GetLastOutput();
        result.Should().ContainSingle(s => s.Contains("NewName, This is the new description, Condition"));
    }

    [Fact]
    public void TestUpdateCommand_Updates_Condition_Updated_Value_Is_Stored()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();
        StorageRule storageRule = new() { Key = "DeletionCondition", VacuumingCondition = "Condition" };
        AddStorageRule(process, storageRule);

        UpdateStorageRule(process, storageRule,
            new StorageRule()
                { Key = "NewName", VacuumingCondition = Condition, Description = "This is a new description" });
        ListStorageRules(process);
        List<string> result = process.GetLastOutput();
        result.Should().ContainSingle(s => s.Contains($"NewName, This is a new description, {Condition}"));
    }

    [Fact]
    public void TestDeleteCommand_Condition_No_Longer_stored()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();
        StorageRule storageRule = new() { Key = "DeletionCondition", VacuumingCondition = "Condition" };
        AddStorageRule(process, storageRule);

        DeleteStorageRule(process, storageRule);

        ListStorageRules(process);
        List<string> result = process.GetLastOutput();
        result.FindAll(s => s.Contains("DeletionCondition")).Should().BeEmpty();
    }

    [Fact]
    public void TestDeleteCommand_Return_Correct_Output()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        // dcs add --name DeletionCondition -c "Condition"
        StorageRule storageRule = new() { Key = "DeletionCondition", VacuumingCondition = "Condition" };
        AddStorageRule(process, storageRule);

        // dcs d --name DeletionCondition
        DeleteStorageRule(process, storageRule);
        List<string> result = process.GetLastOutput();
        result.First().Should().Contain("Successfully deleted DeletionCondition");
    }

    [Fact]
    public void TestShowCommand_Return_Correct_Output()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();
        StorageRule storageRule = new() { Key = "DeletionCondition", VacuumingCondition = "Condition" };
        AddStorageRule(process, storageRule);

        ShowStorageRule(process, storageRule);
        List<string> result = process.GetLastOutput();
        result.First().Should().Contain("DeletionCondition, , Condition");
    }
}