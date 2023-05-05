using FluentAssertions;
using IntegrationTests.SystemTest.Tools;

namespace IntegrationTests.SystemTest;

[Collection("SystemTestSequential")]
public class ProcessingTests : TestResources
{

    [Fact]
    public void TestAdd_Prints_Correct_Message()
    {
        TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();
        
        AddPurpose(process,TestPurpose);
        AddPersonalData(process,TestPersonalDataColumn);
        AddProcessing(process, TestProcessing);

        List<string> result = process.GetLastOutput();
        result.First().Should().Contain("ProcessingDescription");
        result.First().Should().Contain("ProcessingName");
        result.First().Should().Contain("Successfully");
    }

    [Fact]
    public void TestAdd_Processing_Stored_Correctly()
    {
        TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();
        
        AddPurpose(process,TestPurpose);
        AddPersonalData(process,TestPersonalDataColumn);
        AddProcessing(process, TestProcessing);

        ListProcessing(process);
        List<string> result = process.GetLastOutput();
        result.FindAll(s=>s.Contains(TestProcessing.Name) 
                          && s.Contains(TestProcessing.Description)).Should().ContainSingle();
    }

    [Fact]
    public void TestUpdate_Prints_Correct_Message()
    {
        TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();
        
        AddPurpose(process,TestPurpose);
        AddPersonalData(process,TestPersonalDataColumn);
        AddProcessing(process, TestProcessing);
        
        UpdateProcessing(process,TestProcessing,NewTestProcessing);
        
        List<string> result = process.GetLastOutput();
        result[1].Should().Contain(TestProcessing.Name);
        result[1].Should().Contain(NewTestProcessing.Name);
        result[1].Should().Contain("Successfully");
        result[1].Should().Contain("updated");
    }

    [Fact]
    public void TestUpdate_Processing_Stored_Correctly()
    {
        TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();
        
        AddPurpose(process,TestPurpose);
        AddPersonalData(process,TestPersonalDataColumn);
        AddProcessing(process, TestProcessing);
        
        UpdateProcessing(process,TestProcessing,NewTestProcessing);
        
        ListProcessing(process);
        List<string> result = process.GetLastOutput();
        result.FindAll(s=>s.Contains(NewTestProcessing.Name) 
                          && s.Contains(NewTestProcessing.Description)).Should().ContainSingle();
    }

    [Fact]
    public void TestDelete_Prints_Correct_Message()
    {
        TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();
        
        AddPurpose(process,TestPurpose);
        AddPersonalData(process,TestPersonalDataColumn);
        AddProcessing(process, TestProcessing);
        
        DeleteProcessing(process,TestProcessing);

        List<string> result = process.GetLastOutput();
        result.First().Should().Contain("deleted");
        result.First().Should().Contain(TestProcessing.Name);

    }

    [Fact]
    public void TestDelete_Removes_Processing_From_System()
    {
        TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();
        
        AddPurpose(process,TestPurpose);
        AddPersonalData(process,TestPersonalDataColumn);
        AddProcessing(process, TestProcessing);
        
        DeleteProcessing(process,TestProcessing);
        
        ListProcessing(process);
        List<string> result = process.GetLastOutput();
        result.FindAll(s=>s.Contains(TestProcessing.Name) 
                          && s.Contains(TestProcessing.Description)).Should().BeEmpty();
    }

    [Fact]
    public void TestShow_Returns_Correct_Value()
    {
        TestProcess process = Tools.SystemTest.CreateTestProcess();
        process.Start();
        
        AddPurpose(process,TestPurpose);
        AddPersonalData(process,TestPersonalDataColumn);
        AddProcessing(process, TestProcessing);

        ShowProcessing(process,TestProcessing);
        List<string> result = process.GetLastOutput();
        result.FindAll(s=>s.Contains(TestProcessing.Name) 
                          && s.Contains(TestProcessing.Description)).Should().ContainSingle();
    }
}