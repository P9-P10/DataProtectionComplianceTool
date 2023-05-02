using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Managers;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;
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

    private const string Description = "This is a description";

    private static readonly DeleteCondition TestDeleteCondition = new()
    {
        Name = "deleteConditionName",
        Description = Description,
        Condition = "This is a condition"
    };

    private static readonly DeleteCondition NewTestDeleteCondition = new()
    {
        Name = TestDeleteCondition.GetName() + "NEW",
        Description = TestDeleteCondition.GetDescription() + "NEW",
        Condition = TestDeleteCondition.GetCondition() + "NEW"
    };

    private static readonly Purpose TestPurpose = new()
    {
        Name = "purposeName",
        Description = Description,
        DeleteCondition = TestDeleteCondition,
        LegallyRequired = true,
        Columns = new List<PersonalDataColumn>(),
        Rules = new List<VacuumingRule>()
    };

    private static readonly Purpose NewTestPurpose = new()
    {
        Name = TestPurpose.GetName() + "NEW",
        Description = Description + "NEW",
        DeleteCondition = NewTestDeleteCondition,
        LegallyRequired = !TestPurpose.GetLegallyRequired(),
        Columns = new List<PersonalDataColumn>(),
        Rules = new List<VacuumingRule>()
    };

    private static void AddDeleteCondition(TestProcess testProcess, IDeleteCondition deleteCondition)
    {
        var addDeleteConditionCommand = $"{CommandNamer.DeleteConditionName} {CommandNamer.Add} " +
                                        $"{OptionNamer.Name} {deleteCondition.GetName()} " +
                                        $"{OptionNamer.Condition} \"{deleteCondition.GetCondition()}\" " +
                                        $"{OptionNamer.Description} \"{deleteCondition.GetDescription()}\"";
        
        testProcess.GiveInput(addDeleteConditionCommand);
    }

    private static void AddPurpose(TestProcess testProcess)
    {
        var addPurposeCommand = $"{CommandNamer.PurposesName} {CommandNamer.Add} " +
                                         $"{OptionNamer.Name} {TestPurpose.GetName()} " +
                                         $"{OptionNamer.Description} \"{TestPurpose.GetDescription()}\" " +
                                         $"{OptionNamer.DeleteConditionName} {TestPurpose.GetDeleteCondition()} " +
                                         $"{OptionNamer.LegallyRequired} {TestPurpose.GetLegallyRequired()} ";

        testProcess.GiveInput(addPurposeCommand);
    }

    private static void ShowPurpose(TestProcess testProcess, IPurpose purpose)
    {
        var showPurposeCommand = $"{CommandNamer.PurposesName} {CommandNamer.Show} " + 
                                 $"{OptionNamer.Name} {purpose.GetName()}";
        
        testProcess.GiveInput(showPurposeCommand);
    }

    private static void UpdatePurpose(TestProcess testProcess)
    {
        var updatePurposeCommand = $"{CommandNamer.PurposesName} {CommandNamer.Update} " +
                                   $"{OptionNamer.Name} {TestPurpose.GetName()} " +
                                   $"{OptionNamer.Description} \"{NewTestPurpose.GetDescription()}\" " +
                                   $"{OptionNamer.LegallyRequired} {NewTestPurpose.GetLegallyRequired()} " +
                                   $"{OptionNamer.DeleteConditionName} {NewTestPurpose.GetDeleteCondition()} " +
                                   $"{OptionNamer.NewName} {NewTestPurpose.GetName()}";
        
        testProcess.GiveInput(updatePurposeCommand);
    }
    
    [Fact]
    public void AddIsSuccessful()
    {
        using var process = SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestDeleteCondition);
        AddPurpose(process);

        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace().ToList();

        error.Should().BeEmpty();
        output.Should().ContainSingle(s => 
            s.Contains($"Successfully added {TestPurpose.GetName()} purpose") &&
            s.Contains($"{TestPurpose.GetLegallyRequired()}") && 
            s.Contains(Description)
            );
        output.Should().ContainSingle(s =>
            s.Contains($"Successfully updated {TestPurpose.GetName()} purpose with {TestDeleteCondition.GetName()}"));
    }

    [Fact]
    public void PurposeCanBeRetrievedAfterAdd()
    {
        using var process = SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestDeleteCondition);
        AddPurpose(process);
        ShowPurpose(process, TestPurpose);
        
        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();
        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains(TestPurpose.ToListing()));
    }

    [Fact]
    public void PurposeCanBeUpdated()
    {
        using var process = SystemTest.CreateTestProcess();
        process.Start();

        AddDeleteCondition(process, TestDeleteCondition);
        AddDeleteCondition(process, NewTestDeleteCondition);
        AddPurpose(process);
        UpdatePurpose(process);
        ShowPurpose(process, NewTestPurpose);
        process.GiveInput("lgs ls");
        
        var error = process.GetAllErrorsNoWhitespace();
        var output = process.GetAllOutputNoWhitespace();
        error.Should().BeEmpty();
        output.Should().ContainSingle(s => s.Contains(NewTestPurpose.ToListing()));
    }
}