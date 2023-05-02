using System.Collections.Generic;
using GraphManipulation.Commands.Helpers;
using GraphManipulation.Models;
using GraphManipulation.Models.Interfaces;

namespace Test.SystemTest;

public class TestResources
{
    protected const string Description = "This is a description";
    protected const string Condition = "This is a condition";

    protected static readonly DeleteCondition TestDeleteCondition = new()
    {
        Name = "deleteConditionName",
        Description = Description,
        Condition = Condition
    };

    protected static readonly DeleteCondition NewTestDeleteCondition = new()
    {
        Name = TestDeleteCondition.GetName() + "NEW",
        Description = TestDeleteCondition.GetDescription() + "NEW",
        Condition = TestDeleteCondition.GetCondition() + "NEW"
    };

    protected static readonly Purpose TestPurpose = new()
    {
        Name = "purposeName",
        Description = Description,
        DeleteCondition = TestDeleteCondition,
        LegallyRequired = true,
        Columns = new List<PersonalDataColumn>(),
        Rules = new List<VacuumingRule>()
    };

    protected static readonly Purpose NewTestPurpose = new()
    {
        Name = TestPurpose.GetName() + "NEW",
        Description = Description + "NEW",
        DeleteCondition = NewTestDeleteCondition,
        LegallyRequired = !TestPurpose.GetLegallyRequired(),
        Columns = new List<PersonalDataColumn>(),
        Rules = new List<VacuumingRule>()
    };
    
    protected static void AddDeleteCondition(TestProcess testProcess, IDeleteCondition deleteCondition)
    {
        var addDeleteConditionCommand = $"{CommandNamer.DeleteConditionName} {CommandNamer.Add} " +
                                        $"{OptionNamer.Name} {deleteCondition.GetName()} " +
                                        $"{OptionNamer.Condition} \"{deleteCondition.GetCondition()}\" " +
                                        $"{OptionNamer.Description} \"{deleteCondition.GetDescription()}\"";
        
        testProcess.GiveInput(addDeleteConditionCommand);
    }

    protected static void AddPurpose(TestProcess testProcess, IPurpose purpose)
    {
        var addPurposeCommand = $"{CommandNamer.PurposesName} {CommandNamer.Add} " +
                                         $"{OptionNamer.Name} {purpose.GetName()} " +
                                         $"{OptionNamer.Description} \"{purpose.GetDescription()}\" " +
                                         $"{OptionNamer.DeleteConditionName} {purpose.GetDeleteCondition()} " +
                                         $"{OptionNamer.LegallyRequired} {purpose.GetLegallyRequired()} ";

        testProcess.GiveInput(addPurposeCommand);
    }

    protected static void ShowPurpose(TestProcess testProcess, IPurpose purpose)
    {
        var showPurposeCommand = $"{CommandNamer.PurposesName} {CommandNamer.Show} " + 
                                 $"{OptionNamer.Name} {purpose.GetName()}";
        
        testProcess.GiveInput(showPurposeCommand);
    }

    protected static void UpdatePurpose(TestProcess testProcess, IPurpose old, IPurpose updated)
    {
        var updatePurposeCommand = $"{CommandNamer.PurposesName} {CommandNamer.Update} " +
                                   $"{OptionNamer.Name} {old.GetName()} " +
                                   $"{OptionNamer.Description} \"{updated.GetDescription()}\" " +
                                   $"{OptionNamer.LegallyRequired} {updated.GetLegallyRequired()} " +
                                   $"{OptionNamer.DeleteConditionName} {updated.GetDeleteCondition()} " +
                                   $"{OptionNamer.NewName} {updated.GetName()}";
        
        testProcess.GiveInput(updatePurposeCommand);
    }

    protected static void ListPurpose(TestProcess testProcess)
    {
        testProcess.GiveInput($"{CommandNamer.PurposesName} {CommandNamer.List}");
    }

    protected static void DeletePurpose(TestProcess testProcess, IPurpose purpose)
    {
        var deleteCommand = $"{CommandNamer.PurposesName} {CommandNamer.Delete} " +
                            $"{OptionNamer.Name} {purpose.GetName()}";
        testProcess.GiveInput(deleteCommand);
    }
}