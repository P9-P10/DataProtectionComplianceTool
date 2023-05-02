using FluentAssertions;
using GraphManipulation.Commands.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.SystemTest;

[Collection("SystemTestSequential")]
public class PurposeTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public PurposeTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    private const string PurposeName1 = "purposeName1";
    private const string DeleteConditionName = "deleteConditionName";
    private const string DeleteCondition = "This is a delete condition";

    private static void AddDeleteCondition(TestProcess testProcess)
    {
        const string addDeleteConditionCommand = $"{CommandNamer.DeleteConditionName} {CommandNamer.Add} " +
                                                 $"{OptionNamer.Name} {DeleteConditionName} " +
                                                 $"{OptionNamer.Condition} \"{DeleteCondition}\" ";
        
        testProcess.GiveInput(addDeleteConditionCommand);
    }

    private static void AddPurpose(TestProcess testProcess)
    {
        const string addPurposeCommand = $"{CommandNamer.PurposesName} {CommandNamer.Add} " +
                                         $"{OptionNamer.Name} {PurposeName1} " +
                                         $"{OptionNamer.DeleteConditionName} {DeleteConditionName}";
        
        testProcess.GiveInput(addPurposeCommand);
    }

    private static void ShowPurpose(TestProcess testProcess)
    {
        const string showPurposeCommand = $"{CommandNamer.PurposesName} {CommandNamer.Show} " +
                                          $"{OptionNamer.Name} {PurposeName1}";
        
        testProcess.GiveInput(showPurposeCommand);
    }
    
    [Fact]
    public void AddIsSuccessful()
    {
        using var process = SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process);
        AddPurpose(process);

        process.GetError().Should().BeEmpty();
        process.GetOutput().Should().Contain($"Successfully added {PurposeName1} purpose");
    }

    [Fact]
    public void PurposeCanBeRetrievedAfterAdd()
    {
        using var process = SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process);
        AddPurpose(process);
        ShowPurpose(process);

        process.GetError().Should().BeEmpty();
    }
}