using FluentAssertions;
using GraphManipulation.Models;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;


[Collection("SystemTestSequential")]
public class DeletionConditionsTest : TestResources
{
    [Fact]
    public void TestAddCommand_Returns_Correct_Message()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process,new DeleteCondition(){Name = "DeletionCondition",Condition = "Condition"});
        string result = process.GetOutput();

        result.Should().Contain("Successfully added DeletionCondition delete condition with , Condition");
    }

    [Fact]
    public void TestAddCommand_Stores_Value()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process,new DeleteCondition(){Name = "DeletionCondition",Condition = "Condition"});
        ListDeletionConditions(process);
        string result = process.GetOutput();

        result.Should().Contain("DeletionCondition, , Condition");
    }


    [Fact]
    public void TestUpdateCommand_Updates_Name_Updated_Value_Is_Stored()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();
        DeleteCondition deleteCondition = new() {Name = "DeletionCondition", Condition = "Condition"};
        AddDeleteCondition(process,deleteCondition);

    
        UpdateDeletionCondition(process,deleteCondition,new DeleteCondition(){Name = "NewName",Condition = Condition});
        
        ListDeletionConditions(process);
        List<string> result = process.GetLastOutput();
        result[1].Should().Contain("NewName, , This is a condition");
    }

    [Fact]
    public void TestUpdateCommand_Updates_Description_Updated_Value_Is_Stored()
    {
        using TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();
        DeleteCondition deleteCondition = new() {Name = "DeletionCondition", Condition = "Condition"};
        AddDeleteCondition(process,deleteCondition);

    
        UpdateDeletionCondition(process,deleteCondition,new DeleteCondition(){Name = "NewName",Condition = "Condition",
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
        DeleteCondition deleteCondition = new() {Name = "DeletionCondition", Condition = "Condition"};
        AddDeleteCondition(process,deleteCondition);

        UpdateDeletionCondition(process,deleteCondition,
            new DeleteCondition(){Name = "NewName",Condition = Condition,Description = "This is a new description"});
        ListDeletionConditions(process);
        string result = process.GetOutput();
        result.Should().Contain("NewName, This is a new description, This is a condition");
    }
    
    [Fact]
    public void TestDeleteCommand_Condition_No_Longer_stored()
    {
        using TestProcess process = IntegrationTests.SystemTest.Tools.SystemTest.CreateTestProcess();
        process.Start();
        DeleteCondition deleteCondition = new() {Name = "DeletionCondition", Condition = "Condition"};
        AddDeleteCondition(process,deleteCondition);

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
        DeleteCondition deleteCondition = new() {Name = "DeletionCondition", Condition = "Condition"};
        AddDeleteCondition(process,deleteCondition);

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
        DeleteCondition deleteCondition = new() {Name = "DeletionCondition", Condition = "Condition"};
        AddDeleteCondition(process,deleteCondition);
        
        ShowDeleteCondition(process,deleteCondition);
        List<string> result = process.GetLastOutput();
        result.First().Should().Contain("DeletionCondition, , Condition");
    }
}