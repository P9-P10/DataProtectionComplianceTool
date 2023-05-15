using FluentAssertions;
using GraphManipulation.Models;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;


public class DeletionConditionsTest : TestResources
{
    [Fact]
    public void TestAddCommand_Returns_Correct_Message()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process,new StorageRule(){Key = "DeletionCondition",VacuumingCondition = "Condition"});
        string result = process.GetOutput();

        result.Should().Contain("Successfully added DeletionCondition delete condition with , Condition");
    }

    [Fact]
    public void TestAddCommand_Stores_Value()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process,new StorageRule(){Key = "DeletionCondition",VacuumingCondition = "Condition"});
        ListDeletionConditions(process);
        string result = process.GetOutput();

        result.Should().Contain("DeletionCondition, , Condition");
    }


    [Fact]
    public void TestUpdateCommand_Updates_Name_Updated_Value_Is_Stored()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();
        StorageRule storageRule = new() {Key = "DeletionCondition", VacuumingCondition = "Condition"};
        AddDeleteCondition(process,storageRule);

    
        UpdateDeletionCondition(process,storageRule,new StorageRule(){Key = "NewName",VacuumingCondition = Condition});
        
        ListDeletionConditions(process);
        List<string> result = process.GetLastOutput();
        result[1].Should().Contain("NewName, , TRUE");
    }

    [Fact]
    public void TestUpdateCommand_Updates_Description_Updated_Value_Is_Stored()
    {
        using TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();
        StorageRule storageRule = new() {Key = "DeletionCondition", VacuumingCondition = "Condition"};
        AddDeleteCondition(process,storageRule);

    
        UpdateDeletionCondition(process,storageRule,new StorageRule(){Key = "NewName",VacuumingCondition = "Condition",
            Description = "This is the new description"});
        ListDeletionConditions(process);
        
        List<string> result = process.GetLastOutput();
        result[1].Should().Contain("NewName, This is the new description, Condition");
    }
    
    [Fact]
    public void TestUpdateCommand_Updates_Condition_Updated_Value_Is_Stored()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();
        StorageRule storageRule = new() {Key = "DeletionCondition", VacuumingCondition = "Condition"};
        AddDeleteCondition(process,storageRule);

        UpdateDeletionCondition(process,storageRule,
            new StorageRule(){Key = "NewName",VacuumingCondition = Condition,Description = "This is a new description"});
        ListDeletionConditions(process);
        string result = process.GetOutput();
        result.Should().Contain("NewName, This is a new description, TRUE");
    }
    
    [Fact]
    public void TestDeleteCommand_Condition_No_Longer_stored()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();
        StorageRule storageRule = new() {Key = "DeletionCondition", VacuumingCondition = "Condition"};
        AddDeleteCondition(process,storageRule);

        DeleteDeletionCondition(process,deleteCondition);
        
        ListDeletionConditions(process);
        List<string> result = process.GetLastOutput();
        result.FindAll(s=>s.Contains("DeletionCondition")).Should().BeEmpty();
    }
    
    [Fact]
    public void TestDeleteCommand_Return_Correct_Output()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();
        
        // dcs add --name DeletionCondition -c "Condition"
        StorageRule storageRule = new() {Key = "DeletionCondition", VacuumingCondition = "Condition"};
        AddDeleteCondition(process,storageRule);

        // dcs d --name DeletionCondition
        DeleteDeletionCondition(process,deleteCondition);
        List<string> result = process.GetLastOutput();
        result.First().Should().Contain("Successfully deleted DeletionCondition");
    }
    
    [Fact]
    public void TestShowCommand_Return_Correct_Output()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();
        StorageRule storageRule = new() {Key = "DeletionCondition", VacuumingCondition = "Condition"};
        AddDeleteCondition(process,storageRule);
        
        ShowDeleteCondition(process,deleteCondition);
        List<string> result = process.GetLastOutput();
        result.First().Should().Contain("DeletionCondition, , Condition");
    }
}