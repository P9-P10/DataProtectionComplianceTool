using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GraphManipulation.Commands.Helpers;
using Xunit;

namespace Test.SystemTest;


[Collection("SystemTestSequential")]
public class TestDeletionConditions
{
    [Fact]
    public void TestAddCommand_Returns_Correct_Message()
    {
        using TestProcess process = SystemTest.CreateTestProcess();
        process.Start();

        process.GiveInput($"{CommandNamer.DeleteConditionAlias} {CommandNamer.Add} {OptionNamer.Name} DeletionCondition {OptionNamer.Condition} \"Condition\"");
        string result = process.GetOutput();

        result.Should().Contain("Successfully added DeletionCondition delete condition with , Condition");
    }

    [Fact]
    public void TestAddCommand_Stores_Value()
    {
        using TestProcess process = SystemTest.CreateTestProcess();
        process.Start();

        process.GiveInput(
            $"{CommandNamer.DeleteConditionAlias} {CommandNamer.Add} {OptionNamer.Name} DeletionCondition {OptionNamer.Condition} \"Condition\"");
        process.GiveInput($"{CommandNamer.DeleteConditionAlias} {CommandNamer.List}");
        string result = process.GetOutput();

        result.Should().Contain("DeletionCondition, , Condition");
    }

    [Fact]
    public void TestUpdateCommand_Updates_Name_Returns_Correct_Message()
    {
        using TestProcess process = SystemTest.CreateTestProcess();
        process.Start();
        process.GiveInput(
            $"{CommandNamer.DeleteConditionAlias} {CommandNamer.Add} {OptionNamer.Name} DeletionCondition {OptionNamer.ConditionAlias} \"Condition\"");

        process.GiveInput(
            $"{CommandNamer.DeleteConditionAlias} {CommandNamer.UpdateAlias} {OptionNamer.NameAlias} DeletionCondition {OptionNamer.NewNameAlias} NewName");
        List<string> result = process.GetLastOutput();
        result.First().Should().Contain("Successfully updated DeletionCondition delete condition with NewName");
    }

    [Fact]
    public void TestUpdateCommand_Updates_Name_Updated_Value_Is_Stored()
    {
        using TestProcess process = SystemTest.CreateTestProcess();
        process.Start();
        process.GiveInput(
            $"{CommandNamer.DeleteConditionAlias} {CommandNamer.Add} {OptionNamer.Name} DeletionCondition {OptionNamer.ConditionAlias} \"Condition\"");

        process.GiveInput(
            $"{CommandNamer.DeleteConditionAlias} {CommandNamer.UpdateAlias} {OptionNamer.NameAlias} DeletionCondition {OptionNamer.NewNameAlias} NewName");
        process.GiveInput($"{CommandNamer.DeleteConditionAlias} {CommandNamer.List}");
        string result = process.GetOutput();
        result.Should().Contain("NewName, , Condition");
    }

    [Fact]
    public void TestUpdateCommand_Updates_Description_Updated_Value_Is_Stored()
    {
        using TestProcess process = SystemTest.CreateTestProcess();
        process.Start();
        process.GiveInput(
            $"{CommandNamer.DeleteConditionAlias} {CommandNamer.Add} {OptionNamer.Name} DeletionCondition {OptionNamer.ConditionAlias} \"Condition\"");

        process.GiveInput($"{CommandNamer.DeleteConditionAlias} {CommandNamer.UpdateAlias} {OptionNamer.Name}" +
                          $" DeletionCondition {OptionNamer.Description} \"This is the new description\"");
        process.GiveInput($"{CommandNamer.DeleteConditionAlias} {CommandNamer.List}");
        string result = process.GetOutput();
        result.Should().Contain("DeletionCondition, This is the new description, Condition");
    }
    
    [Fact]
    public void TestUpdateCommand_Updates_Condition_Updated_Value_Is_Stored()
    {
        using TestProcess process = SystemTest.CreateTestProcess();
        process.Start();
        process.GiveInput(
            $"{CommandNamer.DeleteConditionAlias} {CommandNamer.Add} {OptionNamer.Name} DeletionCondition {OptionNamer.ConditionAlias} \"Condition\"");

        process.GiveInput($"{CommandNamer.DeleteConditionAlias} {CommandNamer.UpdateAlias} {OptionNamer.Name}" +
                          $" DeletionCondition {OptionNamer.Condition} \"NewCondition\"");
        process.GiveInput($"{CommandNamer.DeleteConditionAlias} {CommandNamer.List}");
        string result = process.GetOutput();
        result.Should().Contain("DeletionCondition, , NewCondition");
    }
    
    [Fact]
    public void TestDeleteCommand_Condition_No_Longer_stored()
    {
        using TestProcess process = SystemTest.CreateTestProcess();
        process.Start();
        process.GiveInput(
            $"{CommandNamer.DeleteConditionAlias} {CommandNamer.Add} {OptionNamer.Name} DeletionCondition {OptionNamer.ConditionAlias} \"Condition\"");

        process.GiveInput($"{CommandNamer.DeleteConditionAlias} {CommandNamer.DeleteAlias} {OptionNamer.Name}" +
                          $" DeletionCondition");
        process.GiveInput($"{CommandNamer.DeleteConditionAlias} {CommandNamer.List}");
        string result = process.GetOutput();
        result.Should().NotContain("DeletionCondition, , NewCondition");
    }
    
    [Fact]
    public void TestDeleteCommand_Return_Correct_Output()
    {
        using TestProcess process = SystemTest.CreateTestProcess();
        process.Start();
        process.GiveInput(
            $"{CommandNamer.DeleteConditionAlias} {CommandNamer.Add} {OptionNamer.Name} DeletionCondition {OptionNamer.ConditionAlias} \"Condition\"");

        process.GiveInput($"{CommandNamer.DeleteConditionAlias} {CommandNamer.DeleteAlias} {OptionNamer.Name}" +
                          $" DeletionCondition");
        List<string> result = process.GetLastOutput();
        result.First().Should().Contain("Successfully deleted DeletionCondition");
    }
    
    [Fact]
    public void TestShowCommand_Return_Correct_Output()
    {
        using TestProcess process = SystemTest.CreateTestProcess();
        process.Start();
        process.GiveInput(
            $"{CommandNamer.DeleteConditionAlias} {CommandNamer.Add} {OptionNamer.Name} DeletionCondition {OptionNamer.ConditionAlias} \"Condition\"");
        process.GiveInput($"{CommandNamer.DeleteConditionAlias} {CommandNamer.ShowAlias} -n DeletionCondition");
        List<string> result = process.GetLastOutput();
        result.First().Should().Contain("DeletionCondition, , Condition");
    }
}