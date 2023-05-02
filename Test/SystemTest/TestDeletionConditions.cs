using FluentAssertions;
using GraphManipulation.Commands.Helpers;
using Xunit;

namespace Test.SystemTest;

// TODO sørg for at tests altid køre ekslusivt af hinanden, så de ikke kan komme til at fejle fordi de mangler den samme resource.
public class TestDeletionConditions
{
    [Fact]
    public void TestAddCommand_Returns_Correct_Message()
    {
        using TestProcess process = SystemTest.CreateTestProcess();
        process.Start();

        process.GiveInput("dcs a -n DeletionCondition -c \"Condition\"");
        string result = process.GetOutput();

        result.Should()
            .Be(
                $@"Using config found at {SystemTest.ConfigPath}" +
                "$: Successfully added DeletionCondition entity with , Condition");
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
        string result = process.GetOutput();
        result.Should().Contain("Successfully updated DeletionCondition entity with NewName");
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
}