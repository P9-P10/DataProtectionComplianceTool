using FluentAssertions;
using GraphManipulation.Models;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

public class StoragePoliciesTest : TestResources
{
    [Fact]
    public void TestAddCommand_Returns_Correct_Message()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStoragePolicy(process, new StoragePolicy() { Key = "DeletionCondition", VacuumingCondition = "Condition" });
        var result = process.GetAllOutput();

        result.Should().ContainSingle(s =>
            s.Contains("successfully") && s.Contains("created") && s.Contains("DeletionCondition"));
    }

    [Fact]
    public void TestAddCommand_Stores_Value()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddStoragePolicy(process, new StoragePolicy() { Key = "DeletionCondition", VacuumingCondition = "Condition" });
        ListStoragePolicies(process);
        string result = process.GetOutput();

        result.Should().Contain("DeletionCondition, , Condition");
    }


    [Fact]
    public void TestUpdateCommand_Updates_Name_Updated_Value_Is_Stored()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();
        StoragePolicy storagePolicy = new() { Key = "DeletionCondition", VacuumingCondition = "Condition" };
        AddStoragePolicy(process, storagePolicy);


        UpdateStoragePolicy(process, storagePolicy,
            new StoragePolicy() { Key = "NewName", VacuumingCondition = Condition });

        ListStoragePolicies(process);
        List<string> result = process.GetLastOutput();
        result.Should().ContainSingle(s => s.Contains("NewName, , TRUE"));
    }

    [Fact]
    public void TestUpdateCommand_Updates_Description_Updated_Value_Is_Stored()
    {
        using TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();
        StoragePolicy storagePolicy = new() { Key = "DeletionCondition", VacuumingCondition = "Condition" };
        AddStoragePolicy(process, storagePolicy);


        UpdateStoragePolicy(process, storagePolicy, new StoragePolicy()
        {
            Key = "NewName", VacuumingCondition = "Condition",
            Description = "This is the new description"
        });
        ListStoragePolicies(process);

        List<string> result = process.GetLastOutput();
        result.Should().ContainSingle(s => s.Contains("NewName, This is the new description, Condition"));
    }

    [Fact]
    public void TestUpdateCommand_Updates_Condition_Updated_Value_Is_Stored()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();
        StoragePolicy storagePolicy = new() { Key = "DeletionCondition", VacuumingCondition = "Condition" };
        AddStoragePolicy(process, storagePolicy);

        UpdateStoragePolicy(process, storagePolicy,
            new StoragePolicy()
                { Key = "NewName", VacuumingCondition = Condition, Description = "This is a new description" });
        ListStoragePolicies(process);
        List<string> result = process.GetLastOutput();
        result.Should().ContainSingle(s => s.Contains($"NewName, This is a new description, {Condition}"));
    }

    [Fact]
    public void TestDeleteCommand_Condition_No_Longer_stored()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();
        StoragePolicy storagePolicy = new() { Key = "DeletionCondition", VacuumingCondition = "Condition" };
        AddStoragePolicy(process, storagePolicy);

        DeleteStoragePolicy(process, storagePolicy);

        ListStoragePolicies(process);
        List<string> result = process.GetLastOutput();
        result.FindAll(s => s.Contains("DeletionCondition")).Should().BeEmpty();
    }

    [Fact]
    public void TestDeleteCommand_Return_Correct_Output()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        // dcs add --name DeletionCondition -c "Condition"
        StoragePolicy storagePolicy = new() { Key = "DeletionCondition", VacuumingCondition = "Condition" };
        AddStoragePolicy(process, storagePolicy);

        // dcs d --name DeletionCondition
        DeleteStoragePolicy(process, storagePolicy);
        List<string> result = process.GetLastOutput();
        result.First().Should().Contain("Storage policy 'DeletionCondition' successfully deleted");
        
    }

    [Fact]
    public void TestShowCommand_Return_Correct_Output()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();
        StoragePolicy storagePolicy = new() { Key = "DeletionCondition", VacuumingCondition = "Condition" };
        AddStoragePolicy(process, storagePolicy);

        ShowStoragePolicy(process, storagePolicy);
        List<string> result = process.GetLastOutput();
        result.First().Should().Contain("DeletionCondition, , Condition");
    }
}